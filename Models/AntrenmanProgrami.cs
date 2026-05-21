using System.Collections.Generic;

namespace SporBeslenmeWeb.Models
{
    public class AntrenmanProgrami
    {
        public int Id { get; set; }

        // Bu alanlara boş değer atayarak o altı çizili uyarıyı siliyoruz
        public string Baslik { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public string Seviye { get; set; } = string.Empty;

        public int GunSayisi { get; set; }

        // Liste için de boş bir liste başlatıyoruz
        public List<ProgramVideosu> Videolar { get; set; } = new List<ProgramVideosu>();
    }
}