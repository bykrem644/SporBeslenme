using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SporBeslenmeWeb.Data;
using SporBeslenmeWeb.Hubs;
using SporBeslenmeWeb.Models;
using System;
using System.IO;
using System.Linq;


namespace SporBeslenmeWeb.Controllers
{
    [Authorize]
    public class MakalelerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IHubContext<NotificationHub> _hubContext; // Yeni eklendi

        public MakalelerController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _hubContext = hubContext;
        }

        // 1. Makalelerin Listelendiği Ana Sayfa
        [AllowAnonymous]
        public IActionResult Index()
        {
            // Makaleleri en yeniden en eskiye doğru sıralayarak getirir
            var makaleler = _context.Makaleler.OrderByDescending(m => m.YayinTarihi).ToList();
            return View(makaleler);
        }

        // 2. Makale Okuma (Detay) Sayfası
   
        public IActionResult Detay(int id)
        {
            var makale = _context.Makaleler.Find(id);
            if (makale == null) return NotFound();
            return View(makale);
        }
        // 3. Makale Ekleme Sayfasını Açma (Burası Eksikti!)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Ekle()
        {
            return View();
        }
        // 3. Makale Ekleme Sayfasını Açma
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Ekle(Makale makale, IFormFile KapakGorseli, IFormFile PdfDosyasi)
        {
            // 1. Kapak Fotoğrafı Yükleme İşlemi (Mevcut Kodun)
            if (KapakGorseli != null && KapakGorseli.Length > 0)
            {
                string resimKlasoru = Path.Combine(_hostEnvironment.WebRootPath, "images", "makaleler");
                if (!Directory.Exists(resimKlasoru)) Directory.CreateDirectory(resimKlasoru);

                string dosyaAdi = Guid.NewGuid().ToString() + Path.GetExtension(KapakGorseli.FileName);
                string tamYol = Path.Combine(resimKlasoru, dosyaAdi);
                using (var fileStream = new FileStream(tamYol, FileMode.Create)) { KapakGorseli.CopyTo(fileStream); }
                makale.KapakGorseliUrl = "/images/makaleler/" + dosyaAdi;
            }

            // 2. YENİ: PDF Dosyası Yükleme İşlemi
            if (PdfDosyasi != null && PdfDosyasi.Length > 0 && PdfDosyasi.ContentType == "application/pdf")
            {
                string pdfKlasoru = Path.Combine(_hostEnvironment.WebRootPath, "pdfs");
                if (!Directory.Exists(pdfKlasoru)) Directory.CreateDirectory(pdfKlasoru);

                string pdfDosyaAdi = Guid.NewGuid().ToString() + ".pdf";
                string pdfTamYol = Path.Combine(pdfKlasoru, pdfDosyaAdi);
                using (var fileStream = new FileStream(pdfTamYol, FileMode.Create)) { PdfDosyasi.CopyTo(fileStream); }
                makale.PdfDosyaUrl = "/pdfs/" + pdfDosyaAdi;
            }

            makale.YayinTarihi = DateTime.Now;

            _context.Makaleler.Add(makale);
            _context.SaveChanges();
            _hubContext.Clients.All.SendAsync("YeniBildirimAl", $"Yeni bir makale eklendi: '{makale.Baslik}' Hemen incele!");

            return RedirectToAction("Index");
        }
        // --- DÜZENLEME SAYFASINI AÇMA ---
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Duzenle(int id)
        {
            var makale = _context.Makaleler.Find(id);
            if (makale == null) return NotFound();

            return View(makale);
        }

        // --- DÜZENLENEN VERİLERİ KAYDETME ---
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Duzenle(int id, Makale guncelMakale, IFormFile? YeniKapakGorseli, IFormFile? YeniPdfDosyasi)
        {
            var makale = _context.Makaleler.Find(id);
            if (makale == null) return NotFound();

            // Metin bilgilerini güncelliyoruz
            makale.Baslik = guncelMakale.Baslik;
            makale.Yazar = guncelMakale.Yazar;
            makale.Ozet = guncelMakale.Ozet;
            makale.Icerik = guncelMakale.Icerik;

            // Eğer hoca YENİ BİR FOTOĞRAF seçtiyse eskisini ezip yenisini yüklüyoruz
            if (YeniKapakGorseli != null && YeniKapakGorseli.Length > 0)
            {
                string dosyaAdi = Guid.NewGuid().ToString() + Path.GetExtension(YeniKapakGorseli.FileName);
                string tamYol = Path.Combine(_hostEnvironment.WebRootPath, "images", "makaleler", dosyaAdi);
                using (var stream = new FileStream(tamYol, FileMode.Create)) { YeniKapakGorseli.CopyTo(stream); }
                makale.KapakGorseliUrl = "/images/makaleler/" + dosyaAdi;
            }

            // Eğer hoca YENİ BİR PDF seçtiyse
            if (YeniPdfDosyasi != null && YeniPdfDosyasi.Length > 0)
            {
                string pdfAdi = Guid.NewGuid().ToString() + ".pdf";
                string pdfTamYol = Path.Combine(_hostEnvironment.WebRootPath, "pdfs", pdfAdi);
                using (var stream = new FileStream(pdfTamYol, FileMode.Create)) { YeniPdfDosyasi.CopyTo(stream); }
                makale.PdfDosyaUrl = "/pdfs/" + pdfAdi;
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // --- MAKALE SİLME İŞLEMİ ---
        [Authorize(Roles = "Admin")]
        public IActionResult Sil(int id)
        {
            var makale = _context.Makaleler.Find(id);
            if (makale != null)
            {
                _context.Makaleler.Remove(makale);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}