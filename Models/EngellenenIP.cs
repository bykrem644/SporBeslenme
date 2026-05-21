using System;

namespace SporBeslenmeWeb.Models
{
    public class EngellenenIP
    {
        public int Id { get; set; }
        public string IPAdresi { get; set; }
        public string Sebep { get; set; } // Örn: "Şüpheli giriş denemesi", "Admin tarafından banlandı"
        public DateTime EngellenmeTarihi { get; set; } = DateTime.Now;
    }
}