using System;

namespace SporBeslenmeWeb.Models
{
    public class GuvenlikLog
    {
        public int Id { get; set; }
        public string KullaniciAdi { get; set; } // Kim yaptı?
        public string IPAdresi { get; set; }     // Hangi ağdan/IP'den?
        public string IslemTuru { get; set; }    // "Giriş Yaptı", "Profil Güncelledi" vs.
        public string Detay { get; set; }        // Ekstra notlar
        public DateTime Tarih { get; set; } = DateTime.Now; // Ne zaman?
    }
}