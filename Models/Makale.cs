using System;

namespace SporBeslenmeWeb.Models
{
    public class Makale
    {
        public int Id { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string Ozet { get; set; } = string.Empty;
        public string Icerik { get; set; } = string.Empty;
            public string Yazar { get; set; } = string.Empty;
        public DateTime YayinTarihi { get; set; } = DateTime.Now;
        public string KapakGorseliUrl { get; set; } = string.Empty;     
        public string PdfDosyaUrl { get; set; } = string.Empty; // PDF yüklenirse buraya kaydedeceğiz
    }
}