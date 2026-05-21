using System;

namespace SporBeslenmeWeb.Models
{
    public class VkiGecmisi
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty; // Hangi kullanıcıya ait
        public double Boy { get; set; }
        public double Kilo { get; set; }
        public double VkiSonucu { get; set; }
        public string Durum { get; set; } = string.Empty; // Zayıf, Normal vs.
        public DateTime Tarih { get; set; } = DateTime.Now;
    }
}