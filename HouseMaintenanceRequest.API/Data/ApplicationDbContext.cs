using HouseMaintenanceRequest.API.Models.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HouseMaintenanceRequest.API.Models.Enums;

namespace HouseMaintenanceRequest.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Landlord> Landlords { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyViewRequest> PropertyViewRequests { get; set; }
        public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }
        public DbSet<MaintenanceCompany> MaintenanceCompanies { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ApplicationUser — Tenant / Landlord / MaintenanceCompany 
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Tenant)
                .WithOne()
                .HasForeignKey<ApplicationUser>(u => u.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Landlord)
                .WithOne()
                .HasForeignKey<ApplicationUser>(u => u.LandlordId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.MaintenanceCompany)
                .WithOne()
                .HasForeignKey<ApplicationUser>(u => u.MaintenanceCompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Landlord → Properties (1 - many)
            builder.Entity<Property>()
                .HasOne(p => p.Landlord)
                .WithMany(l => l.Properties)
                .HasForeignKey(p => p.LandlordId)
                .OnDelete(DeleteBehavior.Restrict);

            // Property → Tenant (1 - 1)
            builder.Entity<Property>()
                .HasOne(p => p.Tenant)
                .WithOne(t => t.Property)
                .HasForeignKey<Property>(p => p.TenantId)
                .OnDelete(DeleteBehavior.SetNull);

            // Tenant → MaintenanceRequests (1 - many)
            builder.Entity<MaintenanceRequest>()
                .HasOne(m => m.Tenant)
                .WithMany(t => t.MaintenanceRequests)
                .HasForeignKey(m => m.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Property → MaintenanceRequests (1 - many)
            builder.Entity<MaintenanceRequest>()
                .HasOne(m => m.Property)
                .WithMany(p => p.MaintenanceRequests)
                .HasForeignKey(m => m.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // MaintenanceCompany → MaintenanceRequests (1 - many)
            builder.Entity<MaintenanceRequest>()
                .HasOne(m => m.MaintenanceCompany)
                .WithMany(c => c.MaintenanceRequests)
                .HasForeignKey(m => m.MaintenanceCompanyId)
                .OnDelete(DeleteBehavior.SetNull);

            // PropertyViewRequest relationships
            builder.Entity<PropertyViewRequest>()
                .HasOne(v => v.Tenant)
                .WithMany(t => t.ViewRequests)
                .HasForeignKey(v => v.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PropertyViewRequest>()
                .HasOne(v => v.Property)
                .WithMany()
                .HasForeignKey(v => v.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Notification → ApplicationUser (Recipient)
            builder.Entity<Notification>()
                .HasOne(n => n.RecipientUser)
                .WithMany()
                .HasForeignKey(n => n.RecipientUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
