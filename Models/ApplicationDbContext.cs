using Microsoft.EntityFrameworkCore;

namespace MoneyControl.Models
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions options):base(options)
        {

        }
        public DbSet<Transcation> Transcations { get; set; }
        public DbSet<category> categories { get; set; }
    }
}
