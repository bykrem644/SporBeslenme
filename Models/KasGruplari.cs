using System.ComponentModel.DataAnnotations;

namespace SporBeslenmeWeb.Models
{
    public class KasGruplari
    {
        [Key]
        public int KasGrupID { get; set; }
        public string Ad { get; set; }
        public string Aciklama { get; set; }
    }
}