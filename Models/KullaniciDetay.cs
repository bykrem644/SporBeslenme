using System.ComponentModel.DataAnnotations;

namespace SporBeslenmeWeb.Models
{
    public class KullaniciDetay
    {
        [Key]
        public int Id { get; set; }

        // Sisteme kayıtlı kullanıcının (Identity) benzersiz ID'sini burada tutacağız ki kimin profili olduğunu bilelim
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lütfen yaşınızı girin.")]
        public int Yas { get; set; }

        [Required(ErrorMessage = "Lütfen boyunuzu girin (Örn: 180).")]
        public double Boy { get; set; }

        [Required(ErrorMessage = "Lütfen kilonuzu girin (Örn: 75.5).")]
        public double Kilo { get; set; }

        // İsteğe bağlı alanlar (null olabilir)
        // Eskiden var olan public string? VucutOlculeri { get; set; } SATIRINI TAMAMEN SİL!
        // Onun yerine şunları ekle:

        public double? Omuz { get; set; }
        public double? Gogus { get; set; }
        public double? Bel { get; set; }
        public double? Kol { get; set; }

        // Hastalıklar aynı kalacak (Biz onu arka planda virgülle birleştirip string olarak kaydedeceğiz)
        public string? Hastaliklar { get; set; }

        // Profil Avatarı İçin
        public string? ProfilFotoUrl { get; set; }


        [Required(ErrorMessage = "Lütfen bir hedef seçin.")]
        public string Hedef { get; set; } = string.Empty;// "Kilo Verme", "Kas Kazanımı", "Kondisyon"

        public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;

        // ANTRENMAN BİLGİLERİ
        public string AntrenmanSeviyesi { get; set; } = "Baslangic"; // Baslangic (3 Gün), Ileri (5 Gün)
        public string? OrtopedikRahatsizliklar { get; set; } // Örn: Bel Fıtığı, Menisküs

        // BESLENME BİLGİLERİ
        public string? BeslenmeKisitlamalari { get; set; } // Örn: Gluten Hassasiyeti, İnsülin Direnci
        public string? SevmedigiBesinler { get; set; } // Örn: Tavuk, Balık, Brokoli
    }
}