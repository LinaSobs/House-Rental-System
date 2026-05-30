using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HouseRentalSystem.Models;

namespace HouseRentalSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Add DbSets for our new models
        public DbSet<House> Houses { get; set; }
        public DbSet<RentalApplication> RentalApplications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure House entity
            builder.Entity<House>(entity =>
            {
                entity.HasOne(h => h.Owner)
                      .WithMany()
                      .HasForeignKey(h => h.OwnerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure RentalApplication entity
            builder.Entity<RentalApplication>(entity =>
            {
                entity.HasOne(ra => ra.House)
                      .WithMany(h => h.RentalApplications)
                      .HasForeignKey(ra => ra.HouseId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ra => ra.Tenant)
                      .WithMany()
                      .HasForeignKey(ra => ra.TenantId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}