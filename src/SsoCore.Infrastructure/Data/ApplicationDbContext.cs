using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;
using System.Reflection.Emit;
using SsoCore.Domain.Entities;
using SsoCore.Infrastructure.Data.Identity;

namespace SsoCore.Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserClaim<string>, ApplicationUserRole,
            IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>(options)
    {
        public DbSet<ApplicationUser> ApplicationUsers { get; set;  }
        public DbSet<IdentityUserClaim<string>> ApplicationUserClaims { get; set;  }
        public DbSet<ApplicationURL> ApplicationURLs { get; set; }
        public DbSet<OpenIddictEntityFrameworkCoreApplication> Clients { get; set; }
        public DbSet<ClientUser> ClientUsers { get; set; }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<ApplicationUserRole> AppliationUserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.UseOpenIddict();
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            builder.Entity<ApplicationUser>(_ =>
            {
                _.ToTable("AspNetUsers");
                _.HasMany(u => u.UserClaims)
                   .WithOne()
                   .HasForeignKey(c => c.UserId)
                   .IsRequired();
            });

            builder.Entity<IdentityUserClaim<string>>(_ =>
            {
                _.ToTable("AspNetUserClaims");
            });
            builder.Entity<OpenIddictEntityFrameworkCoreApplication>(_ =>
            {
                _.ToTable("OpenIddictApplications");
            });

            builder.Entity<ApplicationRole>(_ =>
            {
                _.ToTable("AspNetRoles");
            });

            builder.Entity<ApplicationUserRole>(_ =>
            {
                _.ToTable("AspNetUserRoles");
                _.HasIndex(x => new { x.UserId, x.RoleId, x.ClientId }).IsUnique();
                _.Property(x => x.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

                _.HasOne(ur => ur.User)
                    .WithMany(_=>_.UserRoles)
                    .HasForeignKey(ur => ur.UserId);

                _.HasOne(ur => ur.Role)
                    .WithMany(_ => _.UserRoles) 
                    .HasForeignKey(ur => ur.RoleId);
            });
        }
    }
}
