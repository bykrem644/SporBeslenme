using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SporBeslenmeWeb.Data;
using SporBeslenmeWeb.Models;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Nodes;
using SporBeslenmeWeb.Models;
using System.Xml.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace SporBeslenmeWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        // CONSTRUCTOR (BAŞLANGIÇ) KISMINI ŞÖYLE GÜNCELLE:
        public HomeController(ILogger<HomeController> logger,
                              ApplicationDbContext context,
                              UserManager<IdentityUser> userManager,
                              IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            // Ana sayfa artık API beklemiyor, anında açılacak!
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Haberler()
        {
            try
            {
                var haberlerListesi = new List<SporHaberi>();

                // Şifre yok, limit yok! TRT Spor'un resmi canlı haber akışı
                string rssUrl = "https://www.fotomac.com.tr/rss/anasayfa.xml";

                using var client = new HttpClient();
                // İnternet sitelerinin engellememesi için tarayıcı gibi davranıyoruz
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

                // Veriyi çek
                string xmlResponse = await client.GetStringAsync(rssUrl);

                // Gelen metni XML olarak çözümle
                XDocument xmlDoc = XDocument.Parse(xmlResponse);

                // İçindeki <item> (haber) etiketlerinden ilk 8 tanesini al
                var articles = xmlDoc.Descendants("item").Take(8);

                foreach (var item in articles)
                {
                    haberlerListesi.Add(new SporHaberi
                    {
                        // XML etiketlerinin içindeki yazıları alıyoruz
                        Baslik = item.Element("title")?.Value ?? "Başlıksız Haber",
                        Aciklama = item.Element("description")?.Value ?? "Haberin detayını okumak için tıklayın.",
                        Url = item.Element("link")?.Value ?? "#",
                        Kaynak = "TRT Spor" // Kaynak zaten TRT
                    });
                }

                ViewBag.Haberler = haberlerListesi;
            }
            catch (Exception ex)
            {
                ViewBag.Hata = "Haberler yüklenirken bir sorun oluştu: " + ex.Message;
                ViewBag.Haberler = new List<SporHaberi>();
            }

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
        [HttpGet]
        public IActionResult VkiHesapla()
        {
            // Sayfa ilk açıldığında, eğer kullanıcı giriş yapmışsa eski hesaplamalarını getir
            if (User.Identity!.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
                ViewBag.Gecmis = _context.VkiGecmisleri
                                         .Where(x => x.UserId == userId)
                                         .OrderByDescending(x => x.Tarih)
                                         .ToList();
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VkiHesapla(double boy, double kilo, string cinsiyet, double bel, double boyun, double kalca = 0)
        {
            // --- SENİN MATEMATİKSEL KODLARIN (HİÇ DOKUNULMADI) ---
            double boyMetre = boy / 100.0;
            double vki = kilo / (boyMetre * boyMetre);
            double yagOrani = 0;

            try
            {
                if (cinsiyet == "Erkek")
                {
                    yagOrani = 495.0 / (1.0324 - 0.19077 * Math.Log10(bel - boyun) + 0.15456 * Math.Log10(boy)) - 450.0;
                }
                else if (cinsiyet == "Kadin")
                {
                    yagOrani = 495.0 / (1.29579 - 0.35004 * Math.Log10(bel + kalca - boyun) + 0.22100 * Math.Log10(boy)) - 450.0;
                }
            }
            catch { yagOrani = 0; }

            // Durumu hem ViewBag'e hem de veritabanına yazmak için bir değişkene alıyoruz
            string durumMetni = "Obez";
            if (vki < 18.5) durumMetni = "Zayıf";
            else if (vki < 24.9) durumMetni = "Normal";
            else if (vki < 29.9) durumMetni = "Fazla Kilolu";

            ViewBag.Vki = Math.Round(vki, 1);
            ViewBag.YagOrani = Math.Round(yagOrani, 1);
            ViewBag.Cinsiyet = cinsiyet;
            ViewBag.Durum = durumMetni;

            // --- YENİ: GEÇMİŞİ VERİTABANINA KAYDETME ---
            if (User.Identity!.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);

                var yeniKayit = new VkiGecmisi
                {
                    UserId = userId,
                    Boy = boy,
                    Kilo = kilo,
                    VkiSonucu = Math.Round(vki, 1),
                    Durum = durumMetni,
                    Tarih = DateTime.Now
                };

                _context.VkiGecmisleri.Add(yeniKayit);
                await _context.SaveChangesAsync();

                // Kaydettikten sonra güncel listeyi sayfaya gönderiyoruz
                ViewBag.Gecmis = _context.VkiGecmisleri
                                         .Where(x => x.UserId == userId)
                                         .OrderByDescending(x => x.Tarih)
                                         .ToList();
            }

            return View();
        }
        [HttpGet]
        public IActionResult KaloriHesapla()
        {
            // Sayfa açıldığında eski kalori geçmişini getir
            if (User.Identity!.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
                ViewBag.Gecmis = _context.KaloriGecmisleri
                                         .Where(x => x.UserId == userId)
                                         .OrderByDescending(x => x.Tarih)
                                         .ToList();
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> KaloriHesapla(double boy, double kilo, int yas, string cinsiyet, double aktivite, string hedef)
        {
            // --- SENİN MATEMATİKSEL KODLARIN (HİÇ DOKUNULMADI) ---
            double bmr = (10 * kilo) + (6.25 * boy) - (5 * yas);
            bmr = (cinsiyet == "Erkek") ? bmr + 5 : bmr - 161;

            double gercekAktivite = aktivite / 1000.0;
            double tdee = bmr * gercekAktivite;

            double gunlukKalori = tdee;
            if (hedef == "Zayiflama") gunlukKalori -= 500;
            else if (hedef == "Hacim") gunlukKalori += 400;

            double protein = kilo * 2.2;
            double yag = kilo * 0.9;
            double karb = (gunlukKalori - ((protein * 4) + (yag * 9))) / 4;
            if (karb < 0) karb = 0;

            ViewBag.Kalori = Math.Round(gunlukKalori);
            ViewBag.Protein = Math.Round(protein);
            ViewBag.Yag = Math.Round(yag);
            ViewBag.Karb = Math.Round(karb);
            ViewBag.HedefStr = hedef == "Zayiflama" ? "Yağ Yakımı (Definisyon)" : (hedef == "Hacim" ? "Kas Kazanımı (Bulking)" : "Kilo Koruma");

            // --- YENİ: GEÇMİŞİ VERİTABANINA KAYDETME ---
            if (User.Identity!.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);

                var yeniKayit = new KaloriGecmisi
                {
                    UserId = userId,
                    GunlukIhtiyac = Math.Round(gunlukKalori),
                    Hedef = ViewBag.HedefStr,
                    Tarih = DateTime.Now
                };

                _context.KaloriGecmisleri.Add(yeniKayit);
                await _context.SaveChangesAsync();

                // Güncel tabloyu View'a gönder
                ViewBag.Gecmis = _context.KaloriGecmisleri
                                         .Where(x => x.UserId == userId)
                                         .OrderByDescending(x => x.Tarih)
                                         .ToList();
            }

            return View();
        }
       [Authorize]
public async Task<IActionResult> SanaOzel()
{
    var user = await _userManager.GetUserAsync(User);
    var profil = _context.KullaniciDetaylari.FirstOrDefault(x => x.UserId == user!.Id);

    if (profil == null) return RedirectToAction("Kurulum", "Account");

    // ==========================================
    // 1. EGZERSİZ FİLTRESİ (Hastalık ve Hedef)
    // ==========================================
    var tumEgzersizler = _context.Egzersizler.ToList();
    var kasGruplari = _context.KasGruplari.ToList(); // YENİ: Kas Gruplarını veritabanından çekiyoruz
    
    var uygunEgzersizler = tumEgzersizler.Where(x => x.HedefKitle == profil.Hedef || x.HedefKitle == "Genel").ToList();

    if (!string.IsNullOrEmpty(profil.Hastaliklar))
    {
        var kullaniciHastaliklari = profil.Hastaliklar.Split(',').Select(h => h.Trim().ToLower()).ToList();
        uygunEgzersizler = uygunEgzersizler.Where(e => 
            string.IsNullOrEmpty(e.RiskliDurumlar) || 
            !e.RiskliDurumlar.Split(',').Any(risk => kullaniciHastaliklari.Contains(risk.Trim().ToLower()))
        ).ToList();
    }

    // ==========================================
    // 2. İDMAN BÖLÜCÜ (ANATOMİK YAPAY ZEKA)
    // ==========================================
    var gunlukProgram = new Dictionary<string, List<Egzersizler>>();
    int gunSayisi = profil.AntrenmanSeviyesi == "Baslangic" ? 3 : (profil.AntrenmanSeviyesi == "Orta" ? 4 : 5);
    
    // Günlerin iskeletini oluştur
    for (int i = 1; i <= gunSayisi; i++)
    {
        string gunAdi = gunSayisi == 3 ? (i == 1 ? "1. Gün (Tüm Vücut)" : i == 2 ? "2. Gün (Tüm Vücut)" : "3. Gün (Tüm Vücut)") :
                        gunSayisi == 4 ? (i == 1 ? "1. Gün (Üst Vücut)" : i == 2 ? "2. Gün (Alt Vücut)" : i == 3 ? "3. Gün (Üst Vücut)" : "4. Gün (Alt Vücut)") :
                        (i == 1 ? "1. Gün (Göğüs & Arka Kol)" : i == 2 ? "2. Gün (Sırt & Pazu)" : i == 3 ? "3. Gün (Bacak & Kalf)" : i == 4 ? "4. Gün (Omuz & Karın)" : "5. Gün (Fonksiyonel & Kardiyo)");
        
        gunlukProgram.Add(gunAdi, new List<Egzersizler>());
    }

    // Hareketleri Kas Grubuna Göre Hedef Güne Yerleştir
    var rastgeleEgzersizler = uygunEgzersizler.OrderBy(x => Guid.NewGuid()).ToList();
    
    for (int i = 0; i < rastgeleEgzersizler.Count; i++)
    {
        var egzersiz = rastgeleEgzersizler[i];

                // Egzersizin kas grubunu buluyoruz (ID eşleşmesi ile)
                // NOT: Eğer KasGruplari tablosundaki ID'nin adı KasGroupID yerine Id ise burayı k.Id == egzersiz.KasGrupID olarak değiştir!
                var kasGrup = kasGruplari.FirstOrDefault(k => k.KasGrupID == egzersiz.KasGrupID);
                string kasGrupAdi = kasGrup != null && !string.IsNullOrEmpty(kasGrup.Ad) ? kasGrup.Ad.ToLower() : "";

        int hedefGunIndex = 0;

        if (gunSayisi == 5) // 5 Günlük Bölgesel Split
        {
            if (kasGrupAdi.Contains("göğüs") || kasGrupAdi.Contains("gogus") || kasGrupAdi.Contains("arka kol") || kasGrupAdi.Contains("triceps"))
                hedefGunIndex = 0; // 1. Gün
            else if (kasGrupAdi.Contains("sırt") || kasGrupAdi.Contains("sirt") || kasGrupAdi.Contains("pazu") || kasGrupAdi.Contains("biceps") || kasGrupAdi.Contains("kol"))
                hedefGunIndex = 1; // 2. Gün
            else if (kasGrupAdi.Contains("bacak") || kasGrupAdi.Contains("kalf") || kasGrupAdi.Contains("calf") || kasGrupAdi.Contains("kalça"))
                hedefGunIndex = 2; // 3. Gün
            else if (kasGrupAdi.Contains("omuz") || kasGrupAdi.Contains("karın") || kasGrupAdi.Contains("karin") || kasGrupAdi.Contains("abs"))
                hedefGunIndex = 3; // 4. Gün
            else
                hedefGunIndex = 4; // 5. Gün
        }
        else if (gunSayisi == 4) // 4 Günlük Alt/Üst Split
        {
            bool altVucutMu = kasGrupAdi.Contains("bacak") || kasGrupAdi.Contains("kalf") || kasGrupAdi.Contains("calf") || kasGrupAdi.Contains("kalça");
            // Alt vücut ise 2. veya 4. güne, Üst vücut ise 1. veya 3. güne dengeli dağıt
            hedefGunIndex = altVucutMu ? ((i % 2 == 0) ? 1 : 3) : ((i % 2 == 0) ? 0 : 2);
        }
        else // 3 Günlük Tüm Vücut
        {
            // Tüm vücut olduğu için 3 güne eşit ve adil şekilde dağıtıyoruz
            hedefGunIndex = i % 3; 
        }

        gunlukProgram.ElementAt(hedefGunIndex).Value.Add(egzersiz);
    }

    // ==========================================
    // 3. MUTFAK FİLTRESİ
    // ==========================================
    var tumTarifler = _context.Tarifler.ToList();
    var uygunTarifler = tumTarifler.ToList();

    if (!string.IsNullOrEmpty(profil.BeslenmeKisitlamalari))
    {
        var alerjiler = profil.BeslenmeKisitlamalari.Split(',').Select(a => a.Trim().ToLower()).Where(a => !string.IsNullOrEmpty(a)).ToList();
        uygunTarifler = uygunTarifler.Where(t => 
            string.IsNullOrEmpty(t.IcerdigiAlerjenler) || 
            !alerjiler.Any(a => t.IcerdigiAlerjenler.ToLower().Contains(a))
        ).ToList();
    }

    if (!string.IsNullOrEmpty(profil.SevmedigiBesinler))
    {
        var sevilmeyenler = profil.SevmedigiBesinler.Split(',').Select(s => s.Trim().ToLower()).Where(s => !string.IsNullOrEmpty(s)).ToList();
        uygunTarifler = uygunTarifler.Where(t => 
            !sevilmeyenler.Any(s => 
                (!string.IsNullOrEmpty(t.Ad) && t.Ad.ToLower().Contains(s)) ||
                (!string.IsNullOrEmpty(t.AnaMalzemeler) && t.AnaMalzemeler.ToLower().Contains(s)) ||
                (!string.IsNullOrEmpty(t.Malzemeler) && t.Malzemeler.ToLower().Contains(s))
            )
        ).ToList();
    }

    var gosterilecekTarifler = uygunTarifler.OrderBy(x => Guid.NewGuid()).Take(4).ToList();

    // ==========================================
    // 4. GÜNÜN MAKALEYSİ
    // ==========================================
    var gununMakalesi = _context.Makaleler.OrderBy(x => Guid.NewGuid()).FirstOrDefault();

    // ==========================================
    // 5. VKİ HESAPLAMA
    // ==========================================
    double vki = 0;
    string durum = "Belirtilmedi";
    if (profil.Boy > 0 && profil.Kilo > 0)
    {
        vki = profil.Kilo / ((profil.Boy / 100) * (profil.Boy / 100));
        durum = vki < 18.5 ? "Zayıf" : vki < 25 ? "Normal" : "Kilolu";
    }

    var viewModel = new OnerilerViewModel
    {
        GunlukProgram = gunlukProgram,
        Tarifler = gosterilecekTarifler,
        GununMakalesi = gununMakalesi,
        VKI = Math.Round(vki, 1),
        Durum = durum
    };

    return View(viewModel);
}
        [HttpPost]
        public async Task<JsonResult> AsistanaSor([FromBody] string kullaniciMesaji)
        {
            if (string.IsNullOrWhiteSpace(kullaniciMesaji))
                return Json(new { mesaj = "Lütfen bana bir soru sorun." });

            try
            {
                string apiKey = _configuration["GeminiApiKey"];

                // KONTROL 1: API Key gerçekten okunuyor mu?
                if (string.IsNullOrEmpty(apiKey))
                    return Json(new { mesaj = "SİSTEM HATASI: API Key bulunamadı! Lütfen appsettings.json dosyasını kontrol et." });

                string apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.5-flash:generateContent?key={apiKey}";

                // JSON serileştirme hatalarını sıfıra indiren manuel string formatı
                string prompt = $"Sen SporBeslenme isimli uygulamanın spor koçusun. Soru: {kullaniciMesaji}. Kısa ve net cevap ver.";
                string jsonBody = "{\"contents\":[{\"parts\":[{\"text\":\"" + prompt.Replace("\"", "\\\"").Replace("\n", " ") + "\"}]}]}";

                // TRT'deki gibi SSL sertifika engelini aşan kod
                var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true };
                using var client = new HttpClient(handler);

                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    using JsonDocument doc = JsonDocument.Parse(jsonResponse);
                    string aiCevabi = doc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();
                    return Json(new { mesaj = aiCevabi });
                }
                else
                {
                    // KONTROL 2: Google bizi reddederse GERÇEK SEBEBİ ekrana yazdıralım
                    string hataDetayi = await response.Content.ReadAsStringAsync();
                    return Json(new { mesaj = $"API Hatası ({response.StatusCode}): {hataDetayi}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { mesaj = "Sistem Çöktü: " + ex.Message });
            }
        }
    }
}