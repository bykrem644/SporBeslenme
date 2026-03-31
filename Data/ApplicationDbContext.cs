using Microsoft.EntityFrameworkCore;
using SporBeslenmeWeb.Models;

namespace SporBeslenmeWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<KasGruplari> KasGruplari { get; set; }
        public DbSet<Egzersizler> Egzersizler { get; set; }
        public DbSet<Tarifler> Tarifler { get; set; }
    }
}