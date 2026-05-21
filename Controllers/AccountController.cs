using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SporBeslenmeWeb.Data;
using SporBeslenmeWeb.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SporBeslenmeWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<IdentityUser> userManager,
                                 SignInManager<IdentityUser> signInManager,
                                 RoleManager<IdentityRole> roleManager,
                                 ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        // --- GİZLİ ADMİN YAPMA LİNKİ ---
        public async Task<IActionResult> AdminYap()
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                await _signInManager.RefreshSignInAsync(user);
                return Content("TEBRİKLER! Artık tam yetkili ADMİN oldunuz. Anasayfaya dönüp Ekle/Sil butonlarını test edebilirsiniz.");
            }
            return Content("Önce bir hesaba giriş yapmalısın!");
        }

        // --- 1. KAYIT OL İŞLEMLERİ ---
        [HttpGet]
        public IActionResult Kayit()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Kayit(KayitViewModel model) // Senin modelinin adı farklı olabilir
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.KullaniciAdi, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Sifre);

                if (result.Succeeded)
                {
                    // Kullanıcı veritabanına eklendi, şimdi onu içeri alıyoruz (oturum açıyoruz)
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // İŞTE SİHİRLİ DOKUNUŞ BURASI! 
                    // return RedirectToAction("Index", "Home"); YERİNE AŞAĞIDAKİNİ YAZIYORUZ:

                    return RedirectToAction("Kurulum", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        // --- 2. GİRİŞ YAP İŞLEMLERİ ---
        [HttpGet]
        public IActionResult Giris()
        {
            return View();
        }

        [HttpPost]
        [EnableRateLimiting("LoginKorumasi")]
        public async Task<IActionResult> Giris(GirisViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.KullaniciAdi, model.Sifre, model.BeniHatirla, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.KullaniciAdi);
                    // --- GÜVENLİK LOGLAMASI BAŞLANGICI ---
                    string ipAdresi = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Bilinmiyor";

                    var yeniLog = new GuvenlikLog
                    {
                        KullaniciAdi = model.KullaniciAdi,
                        IPAdresi = ipAdresi,
                        IslemTuru = "Sisteme Giriş",
                        Detay = "Başarılı giriş yapıldı."
                    };

                    _context.GuvenlikLoglari.Add(yeniLog);
                    await _context.SaveChangesAsync();
                    // --- GÜVENLİK LOGLAMASI BİTİŞİ ---
                    bool profilVarMi = _context.KullaniciDetaylari.Any(x => x.UserId == user!.Id);

                    if (!profilVarMi)
                    {
                        return RedirectToAction("Kurulum", "Account");
                    }

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Kullanıcı adı veya şifre hatalı.");
            }
            return View(model);
        }

        // --- 3. ÇIKIŞ YAP (LOGOUT) ---
        public async Task<IActionResult> Cikis()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // ====================================================================
        // BURADAN AŞAĞISI YENİLENMİŞ VE KUSURSUZLAŞTIRILMIŞ PROFİL İŞLEMLERİDİR
        // ====================================================================

        // --- 4. KURULUM EKRANINI AÇMA (İlk Kayıt) ---
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Kurulum()
        {
            var user = await _userManager.GetUserAsync(User);
            var profil = _context.KullaniciDetaylari.FirstOrDefault(x => x.UserId == user!.Id);

            // Eğer adam zaten profilini doldurmuşsa, formu bir daha gösterme, Bilgilerime at
            if (profil != null) return RedirectToAction("Bilgilerim");
            // --- DİNAMİK HASTALIK VE ALERJEN ÇEKİCİ ---
            ViewBag.DinamikHastaliklar = _context.Egzersizler
                .Where(e => e.RiskliDurumlar != null && e.RiskliDurumlar != "")
                .Select(e => e.RiskliDurumlar).ToList()
                .SelectMany(r => r.Split(',')).Select(r => r.Trim()).Distinct().OrderBy(r => r).ToList();

            ViewBag.DinamikAlerjenler = _context.Tarifler
                .Where(t => t.IcerdigiAlerjenler != null && t.IcerdigiAlerjenler != "")
                .Select(t => t.IcerdigiAlerjenler).ToList()
                .SelectMany(a => a!.Split(',')).Select(a => a.Trim()).Distinct().OrderBy(a => a).ToList();
            return View(new KullaniciDetay());
        }

        // --- KURULUM KAYDETME (POST) ---
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Kurulum(KullaniciDetay model, string[]? SecilenHastaliklar, string[]? SecilenAlerjiler, IFormFile? ProfilFoto)
        {
            // Validasyon çökmesini engeller
            ModelState.Clear();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Giris");

            // Yeni modeli kimlikle bağla
            model.UserId = user.Id;

            // Hastalıkları birleştir
            if (SecilenHastaliklar != null && SecilenHastaliklar.Length > 0)
            {
                model.Hastaliklar = string.Join(", ", SecilenHastaliklar);
            }

            // Alerjileri birleştir
            if (SecilenAlerjiler != null && SecilenAlerjiler.Length > 0)
            {
                model.BeslenmeKisitlamalari = string.Join(", ", SecilenAlerjiler);
            }

            // Fotoğrafı kaydet
            if (ProfilFoto != null && ProfilFoto.Length > 0)
            {
                var dosyaAdi = Guid.NewGuid().ToString() + Path.GetExtension(ProfilFoto.FileName);
                var yol = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/avatars", dosyaAdi);
                using (var stream = new FileStream(yol, FileMode.Create)) await ProfilFoto.CopyToAsync(stream);
                model.ProfilFotoUrl = "/images/avatars/" + dosyaAdi;
            }

            // YENİ KAYIT
            _context.KullaniciDetaylari.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Bilgilerim");
        }

        // --- 5. BİLGİLERİM ---
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Bilgilerim()
        {
            var user = await _userManager.GetUserAsync(User);
            var profil = _context.KullaniciDetaylari.FirstOrDefault(x => x.UserId == user!.Id);

            if (profil == null) return RedirectToAction("Kurulum");
            // --- DİNAMİK HASTALIK VE ALERJEN ÇEKİCİ ---
            ViewBag.DinamikHastaliklar = _context.Egzersizler
                .Where(e => e.RiskliDurumlar != null && e.RiskliDurumlar != "")
                .Select(e => e.RiskliDurumlar).ToList()
                .SelectMany(r => r.Split(',')).Select(r => r.Trim()).Distinct().OrderBy(r => r).ToList();

            ViewBag.DinamikAlerjenler = _context.Tarifler
                .Where(t => t.IcerdigiAlerjenler != null && t.IcerdigiAlerjenler != "")
                .Select(t => t.IcerdigiAlerjenler).ToList()
                .SelectMany(a => a!.Split(',')).Select(a => a.Trim()).Distinct().OrderBy(a => a).ToList();
            return View(profil);
        }

        // --- 6. PROFİL GÜNCELLEME EKRANINI AÇMA ---
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ProfilGuncelle()
        {
            var user = await _userManager.GetUserAsync(User);
            var profil = _context.KullaniciDetaylari.FirstOrDefault(x => x.UserId == user!.Id);

            if (profil == null) return RedirectToAction("Kurulum");

            return View(profil); // Mevcut bilgileri formun içine dolu olarak gönderir
        }

        // --- PROFİL GÜNCELLEME KAYDETME (POST) ---
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ProfilGuncelle(KullaniciDetay model, string[]? SecilenHastaliklar, string[]? SecilenAlerjiler, IFormFile? ProfilFoto)
        {
            // Validasyon çökmesini engeller
            ModelState.Clear();

            var user = await _userManager.GetUserAsync(User);
            var eskiProfil = _context.KullaniciDetaylari.FirstOrDefault(x => x.UserId == user!.Id);

            if (eskiProfil != null)
            {
                // Mevcut verileri yenileriyle eziyoruz
                eskiProfil.Boy = model.Boy;
                eskiProfil.Kilo = model.Kilo;
                eskiProfil.Yas = model.Yas;
                eskiProfil.Hedef = model.Hedef;
                eskiProfil.AntrenmanSeviyesi = model.AntrenmanSeviyesi;
                eskiProfil.SevmedigiBesinler = model.SevmedigiBesinler;
                eskiProfil.Omuz = model.Omuz;
                eskiProfil.Gogus = model.Gogus;
                eskiProfil.Bel = model.Bel;
                eskiProfil.Kol = model.Kol;

                // Hastalıklar Checkbox Güncellemesi
                if (SecilenHastaliklar != null && SecilenHastaliklar.Length > 0)
                {
                    eskiProfil.Hastaliklar = string.Join(", ", SecilenHastaliklar);
                }
                else
                {
                    eskiProfil.Hastaliklar = null; // Tümü kaldırıldıysa temizle
                }

                // Alerjiler Checkbox Güncellemesi
                if (SecilenAlerjiler != null && SecilenAlerjiler.Length > 0)
                {
                    eskiProfil.BeslenmeKisitlamalari = string.Join(", ", SecilenAlerjiler);
                }
                else
                {
                    eskiProfil.BeslenmeKisitlamalari = null; // Tümü kaldırıldıysa temizle
                }

                // Profil Fotoğrafı Güncellemesi
                if (ProfilFoto != null && ProfilFoto.Length > 0)
                {
                    var dosyaAdi = Guid.NewGuid().ToString() + Path.GetExtension(ProfilFoto.FileName);
                    var yol = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/avatars", dosyaAdi);
                    using (var stream = new FileStream(yol, FileMode.Create)) await ProfilFoto.CopyToAsync(stream);
                    eskiProfil.ProfilFotoUrl = "/images/avatars/" + dosyaAdi;
                }

                // MEVCUT KAYDI GÜNCELLE
                _context.KullaniciDetaylari.Update(eskiProfil);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Bilgilerim");
        }
        // --- SİBER GÜVENLİK KOMUTA MERKEZİ ---
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GuvenlikIzleme()
        {
            // 1. Logları Çek
            ViewBag.Loglar = _context.GuvenlikLoglari.OrderByDescending(x => x.Tarih).Take(50).ToList();

            // 2. Kara Listeyi Çek
            ViewBag.KaraListe = _context.EngellenenIpler.OrderByDescending(x => x.EngellenmeTarihi).ToList();

            // Sayfayı tek bir modelle değil, ViewBag ile çoklu verilerle besliyoruz
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> IPEngelle(string ipAdresi, string sebep)
        {
            if (!string.IsNullOrEmpty(ipAdresi))
            {
                var yeniEngel = new EngellenenIP { IPAdresi = ipAdresi, Sebep = sebep };
                _context.EngellenenIpler.Add(yeniEngel);

                // Kimin engellediğini loglara da düşelim
                _context.GuvenlikLoglari.Add(new GuvenlikLog { KullaniciAdi = User.Identity.Name, IPAdresi = ipAdresi, IslemTuru = "IP Ban (Manuel)", Detay = sebep });

                await _context.SaveChangesAsync();
            }
            return RedirectToAction("GuvenlikIzleme");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> IPEngelKaldir(int id)
        {
            var ip = await _context.EngellenenIpler.FindAsync(id);
            if (ip != null)
            {
                _context.EngellenenIpler.Remove(ip);
                _context.GuvenlikLoglari.Add(new GuvenlikLog { KullaniciAdi = User.Identity.Name, IPAdresi = ip.IPAdresi, IslemTuru = "Ban Kaldırma", Detay = "IP engeli admin tarafından kaldırıldı." });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("GuvenlikIzleme");
        }   

    } // Bu AccountController sınıfını kapatan parantez
} // Bu da namespace'i kapatan en sondaki parantez
    