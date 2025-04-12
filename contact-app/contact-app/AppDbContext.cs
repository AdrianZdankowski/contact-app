using contact_app.Entities;
using Microsoft.EntityFrameworkCore;

namespace contact_app
{
    public class AppDbContext : DbContext 
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
