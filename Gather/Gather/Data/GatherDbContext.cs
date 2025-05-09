using Gather.Models;
using Microsoft.EntityFrameworkCore;

namespace Gather.Data
{
    public class GatherDbContext : DbContext
    {
        public GatherDbContext(DbContextOptions<GatherDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } //Användare
        public DbSet<Events> Events { get; set; } //Eventet som användaren skapar
        public DbSet<Applications> Applications { get; set; } //Ansökning för att delta i eventet
    }
}
