using Microsoft.EntityFrameworkCore;
using KonyvtarBackend.Models;

namespace KonyvtarBackend.Data
{
    public class KonyvtarContext : DbContext
    {
        public KonyvtarContext(DbContextOptions<KonyvtarContext> options) : base(options)
        {
        }

        public DbSet<Olvaso> Olvasok { get; set; }
        public DbSet<Konyv> Konyvek { get; set; }
    }
}
