using Microsoft.AspNetCore.Mvc;
using SporBeslenmeWeb.Data;
using SporBeslenmeWeb.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace SporBeslenmeWeb.Controllers
{
    [Authorize]
    public class KasGruplariController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KasGruplariController(ApplicationDbContext context)
        {
            _context = context;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            var kasGruplari = _context.KasGruplari.ToList();
            return View(kasGruplari);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Ekle()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Ekle(KasGruplari kasGrubu)
        {
            if (ModelState.IsValid)
            {
                _context.KasGruplari.Add(kasGrubu);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(kasGrubu);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Sil(int id)
        {
            var kasGrubu = _context.KasGruplari.Find(id);
            if (kasGrubu != null)
            {
                _context.KasGruplari.Remove(kasGrubu);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Duzenle(int id)
        {
            var kasGrubu = _context.KasGruplari.Find(id);
            if (kasGrubu == null)
            {
                return NotFound();
            }
            return View(kasGrubu);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Duzenle(KasGruplari kasGrubu)
        {
            if (ModelState.IsValid)
            {
                _context.KasGruplari.Update(kasGrubu);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(kasGrubu);
        }
    }
}