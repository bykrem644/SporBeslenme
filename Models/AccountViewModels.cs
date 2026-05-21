using System.ComponentModel.DataAnnotations;

namespace SporBeslenmeWeb.Models
{
    // 1. Kayıt Olma Formu İçin Veri Paketi
    public class KayitViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        public string Sifre { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("Sifre", ErrorMessage = "Şifreler birbiriyle uyuşmuyor.")]
        public string SifreTekrar { get; set; } = string.Empty;
    }

    // 2. Giriş Yapma Formu İçin Veri Paketi
    public class GirisViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        public string Sifre { get; set; } = string.Empty;

        public bool BeniHatirla { get; set; }
    }
}