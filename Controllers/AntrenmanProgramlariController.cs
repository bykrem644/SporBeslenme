using Microsoft.AspNetCore.Hosting; // Video yüklemek için gerekli
using Microsoft.AspNetCore.Http; // IFormFile için gerekli
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SporBeslenmeWeb.Data;
using SporBeslenmeWeb.Models;
using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace SporBeslenmeWeb.Controllers
{
    [Authorize]
    public class AntrenmanProgramlariController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IWebHostEnvironment _hostEnvironment = hostEnvironment;

        // Programların listelendiği ana sayfa
        [AllowAnonymous]
        public IActionResult Index()
        {
            var programlar = _context.AntrenmanProgramlari.ToList();
            return View(programlar);
        }

        // Programın içine girince videoların listelendiği detay sayfası
      
        public IActionResult Detay(int id)
        {
            var program = _context.AntrenmanProgramlari
                .Include(p => p.Videolar)
                .FirstOrDefault(p => p.Id == id);

            if (program == null) return NotFound();

            program.Videolar = program.Videolar.OrderBy(v => v.Sira).ToList();
            return View(program);
        }
        [Authorize(Roles = "Admin")]
        // --- YENİ: 1. AŞAMA (ANA PROGRAMI EKLEME) ---
        public IActionResult Ekle()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Ekle(AntrenmanProgrami program)
        {
            _context.AntrenmanProgramlari.Add(program);
            _context.SaveChanges();

            // Program eklendikten sonra sistem bizi otomatik olarak o programa video yükleme sayfasına fırlatacak!
            return RedirectToAction("VideoEkle", new { programId = program.Id });
        }
        [Authorize(Roles = "Admin")]
        // --- YENİ: 2. AŞAMA (PROGRAMA VİDEO YÜKLEME) ---
        public IActionResult VideoEkle(int programId)
        {
            var program = _context.AntrenmanProgramlari.Find(programId);
            if (program == null) return NotFound();

            ViewBag.ProgramId = programId;
            ViewBag.ProgramAdi = program.Baslik;
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult VideoEkle(ProgramVideosu video, IFormFile VideoDosyasi)
        {
            // Tıpkı EgzersizlerController'da yaptığın gibi fiziksel dosyayı sunucuya kaydediyoruz
            if (VideoDosyasi != null && VideoDosyasi.Length > 0)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string dosyaAdi = Guid.NewGuid().ToString() + Path.GetExtension(VideoDosyasi.FileName);
                string videoKlasoru = Path.Combine(wwwRootPath, "videos");

                if (!Directory.Exists(videoKlasoru))
                    Directory.CreateDirectory(videoKlasoru);

                string tamYol = Path.Combine(videoKlasoru, dosyaAdi);
                using (var fileStream = new FileStream(tamYol, FileMode.Create))
                {
                    VideoDosyasi.CopyTo(fileStream);
                }
                video.VideoYolu = "/videos/" + dosyaAdi;
            }

            _context.ProgramVideolari.Add(video);
            _context.SaveChanges();

            // Aynı programa birden fazla video ekleneceği için sayfada kalmaya devam et
            return RedirectToAction("VideoEkle", new { programId = video.AntrenmanProgramiId });
        }
        [Authorize(Roles = "Admin")]
        // --- PROGRAM DÜZENLEME SAYFASINI AÇMA ---
        [HttpGet]
        public IActionResult Duzenle(int id)
        {
            var program = _context.AntrenmanProgramlari.Find(id);
            if (program == null) return NotFound();

            return View(program);
        }
        [Authorize(Roles = "Admin")]
        // --- DÜZENLENEN PROGRAMI KAYDETME ---
        [HttpPost]
        public IActionResult Duzenle(int id, AntrenmanProgrami guncelProgram)
        {
            var program = _context.AntrenmanProgramlari.Find(id);
            if (program == null) return NotFound();

            // Sadece ana bilgileri güncelliyoruz (Videolar ayrı ekleniyor zaten)
            program.Baslik = guncelProgram.Baslik;
            program.Aciklama = guncelProgram.Aciklama;
            program.GunSayisi = guncelProgram.GunSayisi;
            program.Seviye = guncelProgram.Seviye;

            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        // --- PROGRAM SİLME İŞLEMİ ---
        public IActionResult Sil(int id)
        {
            var program = _context.AntrenmanProgramlari.Find(id);
            if (program != null)
            {
                // Entity Framework, programı sildiğinde ona bağlı videoları da otomatik silebilir 
                // (Veritabanındaki Cascade Delete ayarına bağlı olarak)
                _context.AntrenmanProgramlari.Remove(program);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}