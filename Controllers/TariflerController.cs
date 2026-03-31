using Microsoft.AspNetCore.Mvc;
using SporBeslenmeWeb.Data;
using SporBeslenmeWeb.Models;
using System.Linq;

namespace SporBeslenmeWeb.Controllers
{
    public class TariflerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TariflerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var tarifler = _context.Tarifler.ToList();
            return View(tarifler);
        }

        public IActionResult Ekle()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Ekle(Tarifler tarif)
        {
            if (ModelState.IsValid)
            {
                _context.Tarifler.Add(tarif);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tarif);
        }
        // SİLME İŞLEMİ
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
        public IActionResult Duzenle(int id)
        {
            var tarif = _context.Tarifler.Find(id);
            if (tarif == null)
            {
                return NotFound();
            }
            return View(tarif);
        }

        // DÜZENLEME İŞLEMİ (Kaydetme)
        [HttpPost]
        public IActionResult Duzenle(Tarifler tarif)
        {
            if (ModelState.IsValid)
            {
                _context.Tarifler.Update(tarif);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tarif);
        }
    }
}