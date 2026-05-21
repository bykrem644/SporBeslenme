using System.ComponentModel.DataAnnotations;

namespace SporBeslenmeWeb.Models
{
    public class Tarifler
    {
        [Key]
        public int TarifID { get; set; }
        public string? Ad { get; set; }
        public string? Kategori { get; set; }
        public int KarbonhidratYuzdesi { get; set; }
        public int ProteinYuzdesi { get; set; }
        public int YagYuzdesi { get; set; }
        public string? Malzemeler { get; set; }
        public string? Hazirlanis { get; set; }
        public string? GorselYolu { get; set; }
        public string? TarifKategorisi { get; set; } // "YuksekProtein", "DusukKalori", "HacimKazanma"
        public string? IcerdigiAlerjenler { get; set; } // Örn: Gluten, Laktoz
        public string? AnaMalzemeler { get; set; } // Örn: Tavuk, Kırmızı Et, Yulaf

    }
}