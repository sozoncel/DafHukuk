using DafHukuk.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DafHukuk.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Lawyer> Lawyers { get; set; }
        public DbSet<SolutionPartner> SolutionPartners { get; set; }

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            base.OnModelCreating(modelbuilder);

            modelbuilder.Entity<Category>()
                .HasMany(c => c.Posts)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId);

            modelbuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Name_TR = "Etkinlikler",
                    Name_EN = "Events",
                    Name_AR = "فعاليات",
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                },
                new Category
                {
                    Id = 2,
                    Name_TR = "Yayınlar",
                    Name_EN = "Publications",
                    Name_AR = "منشورات",
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                },
                new Category
                {
                    Id = 3,
                    Name_TR = "Hizmetlerimiz",
                    Name_EN = "Our Services",
                    Name_AR = "خدماتنا",
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                }
            );

        }
    }
}
