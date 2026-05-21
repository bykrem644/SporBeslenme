namespace SporBeslenmeWeb.Models
{
    public class SporHaberi
    {
        public string Baslik { get; set; }
        public string Url { get; set; }
        public string Aciklama { get; set; } // Kısa açıklama için eklendi
        public string Kaynak { get; set; }
    }
}