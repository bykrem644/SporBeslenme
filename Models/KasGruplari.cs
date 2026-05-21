using System.ComponentModel.DataAnnotations;

namespace SporBeslenmeWeb.Models
{
    public class KasGruplari
    {
        [Key]
        public int KasGrupID { get; set; }
        public string Ad { get; set; }= string.Empty;
        public string Aciklama { get; set; }= string.Empty;
    }
}