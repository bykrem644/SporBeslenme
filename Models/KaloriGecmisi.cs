using System;

namespace SporBeslenmeWeb.Models
{
    public class KaloriGecmisi
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public double GunlukIhtiyac { get; set; }
        public string Hedef { get; set; } = string.Empty; // Kilo Alma, Verme vs.
        public DateTime Tarih { get; set; } = DateTime.Now;
    }
}