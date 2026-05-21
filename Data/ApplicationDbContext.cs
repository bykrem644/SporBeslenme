using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SporBeslenmeWeb.Models;

namespace SporBeslenmeWeb.Data
{
    // DbContext yerine IdentityDbContext<IdentityUser> kullanıyoruz
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<IdentityUser>(options)
    {

        // Mevcut Tabloların (Eksiksiz)
        public DbSet<KasGruplari> KasGruplari { get; set; }
        public DbSet<Egzersizler> Egzersizler { get; set; }
        public DbSet<Tarifler> Tarifler { get; set; }
        public DbSet<AntrenmanProgrami> AntrenmanProgramlari { get; set; }
        public DbSet<ProgramVideosu> ProgramVideolari { get; set; }
        public DbSet<Makale> Makaleler { get; set; }
        public DbSet<KullaniciDetay> KullaniciDetaylari { get; set; }
        public DbSet<VkiGecmisi> VkiGecmisleri { get; set; }
        public DbSet<KaloriGecmisi> KaloriGecmisleri { get; set; }
        public DbSet<GuvenlikLog> GuvenlikLoglari { get; set; }
        public DbSet<EngellenenIP> EngellenenIpler { get; set; }
    }
}