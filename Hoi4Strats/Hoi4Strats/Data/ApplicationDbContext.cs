using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharedProj;
using SharedProj.DBModels;
using System;

namespace Hoi4Strats.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // Konstruktorn
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<GuideModel> Guides { get; set; }
        public DbSet<ImageModel> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // CFG GuideModel
            modelBuilder.Entity<GuideModel>(entity =>
            {
                entity.ToTable("Guides"); // Name
                entity.HasKey(e => e.Id); // PK
                entity.HasMany(g => g.Pictures)
                      .WithOne() // No backwards navigation
                      .HasForeignKey(i => i.GuideId) // FK
                      .OnDelete(DeleteBehavior.Cascade); // Erase pictures if guides are deleted
            });

            // CFG for ImageModel
            modelBuilder.Entity<ImageModel>(entity =>
            {
                entity.ToTable("Images"); // Name
                entity.HasKey(e => e.ImageId); // PK
                entity.Property(e => e.Content).IsRequired(); // Blob-data is required
                entity.Property(e => e.ContentType).IsRequired(); // MIME-typ is required
            });
        }
    }
}