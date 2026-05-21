using System.ComponentModel.DataAnnotations;

namespace SporBeslenmeWeb.Models
{
    public class Egzersizler
    {
        [Key]
        public int EgzersizID { get; set; }
        public int KasGrupID { get; set; }
        public string? Ad { get; set; }
        public string? NasilYapilir { get; set; }
        public string? ZorlukSeviyesi { get; set; }
        public string? GorselYolu { get; set; }
        public string? VideoYolu { get; set; }
        public string? HedefKitle { get; set; } // "KiloVerme", "KasKazanimi", "Kondisyon"
        public string? RiskliDurumlar { get; set; } // "Bel Fıtığı", "Menisküs" (Hangi durumda bu hareket yapılmamalı?)
    }
}