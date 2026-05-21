using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SporBeslenmeWeb.Data;
using SporBeslenmeWeb.Models;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using SporBeslenmeWeb.Hubs;

namespace SporBeslenmeWeb.Controllers
{
    [Authorize]
    public class TariflerController(ApplicationDbContext context, IHubContext<NotificationHub> hubContext) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IHubContext<NotificationHub> _hubContext; // Yeni eklendi

        [AllowAnonymous]
        public IActionResult Index()
        {
            var tarifler = _context.Tarifler.ToList();
            return View(tarifler);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Ekle()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Ekle(Tarifler tarif)
        {
            // SİSTEMİN BOŞ ID TAKINTISINI DEVRE DIŞI BIRAKIYORUZ
            ModelState.Remove("TarifID");
            ModelState.Remove("Id");

            if (ModelState.IsValid)
            {
                _context.Tarifler.Add(tarif);
                _context.SaveChanges();
                _hubContext.Clients.All.SendAsync("YeniBildirimAl", $"🍽️ Nefis bir tarif eklendi: '{tarif.Ad}' Mutfakta seni bekliyor!");
                return RedirectToAction("Index");
            }

            // EĞER HALA KAYDETMİYORSA, GİZLİ HATAYI YAKALAYIP EKRANA YOLLUYORUZ
            var hatalar = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            ViewBag.GizliHata = string.Join(" | ", hatalar);

            return View(tarif);
        }
        // SİLME İŞLEMİ
        [Authorize(Roles = "Admin")]
        public IActionResult Sil(int id)
        {
            var tarif = _context.Tarifler.Find(id);
            if (tarif != null)
            {
                _context.Tarifler.Remove(tarif);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // DÜZENLEME İŞLEMİ (Sayfayı Açma)
        [Authorize(Roles = "Admin")]
        public IActionResult Duzenle(int id)
        {
            var tarif = _context.Tarifler.Find(id);
            if (tarif == null)
            {
                return NotFound();
            }
            return View(tarif);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Duzenle(Tarifler tarif)
        {
            // DÜZENLEMEDE DE ID TAKINTISINI AŞIYORUZ
            ModelState.Remove("TarifID");
            ModelState.Remove("Id");

            if (ModelState.IsValid)
            {
                _context.Tarifler.Update(tarif);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            // GİZLİ HATAYI YAKALA
            var hatalar = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            ViewBag.GizliHata = string.Join(" | ", hatalar);

            return View(tarif);
        }

        public IActionResult Detay(int? id)
        {
            if (id == null) return NotFound();

            var tarif = _context.Tarifler.FirstOrDefault(m => m.TarifID == id);
            if (tarif == null) return NotFound();

            return View(tarif);
        }
    }
}