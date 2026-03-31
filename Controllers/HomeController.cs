using Microsoft.AspNetCore.Mvc;
using SporBeslenmeWeb.Data;
using System.Linq;

namespace SporBeslenmeWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult KasEgzersizleriniGetir(int id)
        {
            var egzersizler = _context.Egzersizler
                .Where(e => e.KasGrupID == id)
                .Select(e => new {
                    ad = e.Ad,
                    zorlukSeviyesi = e.ZorlukSeviyesi,
                    nasilYapilir = e.NasilYapilir,
                    videoYolu = e.VideoYolu
                }).ToList();

            return Json(egzersizler);
        }
    }
}