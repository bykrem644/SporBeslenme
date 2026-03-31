using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SporBeslenmeWeb.Data;
using SporBeslenmeWeb.Models;
using System;
using System.IO;
using System.Linq;

namespace SporBeslenmeWeb.Controllers
{
    public class EgzersizlerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public EgzersizlerController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            var egzersizler = _context.Egzersizler.ToList();
            return View(egzersizler);
        }

        public IActionResult Ekle()
        {
            ViewBag.KasGruplari = new SelectList(_context.KasGruplari.ToList(), "KasGrupID", "Ad");
            return View();
        }

        [HttpPost]
        public IActionResult Ekle(Egzersizler egzersiz, IFormFile VideoDosyasi)
        {
            if (ModelState.IsValid)
            {
                if (VideoDosyasi != null && VideoDosyasi.Length > 0)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string dosyaAdi = Guid.NewGuid().ToString() + Path.GetExtension(VideoDosyasi.FileName);
                    string videoKlasoru = Path.Combine(wwwRootPath, "videos");

                    if (!Directory.Exists(videoKlasoru))
                    {
                        Directory.CreateDirectory(videoKlasoru);
                    }

                    string tamYol = Path.Combine(videoKlasoru, dosyaAdi);

                    using (var fileStream = new FileStream(tamYol, FileMode.Create))
                    {
                        VideoDosyasi.CopyTo(fileStream);
                    }

                    egzersiz.VideoYolu = "/videos/" + dosyaAdi;
                }

                _context.Egzersizler.Add(egzersiz);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.KasGruplari = new SelectList(_context.KasGruplari.ToList(), "KasGrupID", "Ad", egzersiz.KasGrupID);
            return View(egzersiz);
        }
        // SİLME İŞLEMİ
        public IActionResult Sil(int id)
        {
            var egzersiz = _context.Egzersizler.Find(id);
            if (egzersiz != null)
            {
                // Veritabanından kaydı siliyoruz (Opsiyonel olarak sunucudaki fiziksel mp4 dosyası da silinebilir)
                _context.Egzersizler.Remove(egzersiz);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // DÜZENLEME İŞLEMİ (Sayfayı Açma)
        public IActionResult Duzenle(int id)
        {
            var egzersiz = _context.Egzersizler.Find(id);
            if (egzersiz == null)
            {
                return NotFound();
            }

            // Açılır liste için kas gruplarını tekrar gönderiyoruz ve mevcut olanı seçili hale getiriyoruz
            ViewBag.KasGruplari = new SelectList(_context.KasGruplari.ToList(), "KasGrupID", "Ad", egzersiz.KasGrupID);
            return View(egzersiz);
        }

        // DÜZENLEME İŞLEMİ (Kaydetme)
        [HttpPost]
        public IActionResult Duzenle(Egzersizler egzersiz, IFormFile? YeniVideoDosyasi)
        {
            if (ModelState.IsValid)
            {
                // Eğer formdan YENİ bir video dosyası seçildiyse eskisi yerine bunu yükle
                if (YeniVideoDosyasi != null && YeniVideoDosyasi.Length > 0)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string dosyaAdi = Guid.NewGuid().ToString() + Path.GetExtension(YeniVideoDosyasi.FileName);
                    string videoKlasoru = Path.Combine(wwwRootPath, "videos");
                    string tamYol = Path.Combine(videoKlasoru, dosyaAdi);

                    using (var fileStream = new FileStream(tamYol, FileMode.Create))
                    {
                        YeniVideoDosyasi.CopyTo(fileStream);
                    }
                    // Yeni video yolunu modele ata
                    egzersiz.VideoYolu = "/videos/" + dosyaAdi;
                }
                // Yeni video seçilmediyse, formdaki gizli (hidden) input'tan gelen eski VideoYolu aynen kalır.

                _context.Egzersizler.Update(egzersiz);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.KasGruplari = new SelectList(_context.KasGruplari.ToList(), "KasGrupID", "Ad", egzersiz.KasGrupID);
            return View(egzersiz);
        }
    }
}