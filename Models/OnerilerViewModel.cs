using System.Collections.Generic;

namespace SporBeslenmeWeb.Models
{
    public class OnerilerViewModel
    {
        // Artık düz liste değil, Günleri ve o günün hareketlerini tutan sözlük (Dictionary) yapısı
        public Dictionary<string, List<Egzersizler>> GunlukProgram { get; set; } = new Dictionary<string, List<Egzersizler>>();

        public List<Tarifler> Tarifler { get; set; } = new List<Tarifler>();

        // Günün Makalesi İçin (Makale.cs modelin olduğunu varsayarak)
        public Makale? GununMakalesi { get; set; }

        public double VKI { get; set; }
        public string Durum { get; set; } = string.Empty;
    }
}