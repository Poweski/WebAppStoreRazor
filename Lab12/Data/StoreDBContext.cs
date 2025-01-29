using Lab12.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Lab12.Data
{
    public class StoreDBContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Article> Articles { get; set; } = null!;
        public StoreDBContext(DbContextOptions<StoreDBContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Seed();
        }
    }
}
