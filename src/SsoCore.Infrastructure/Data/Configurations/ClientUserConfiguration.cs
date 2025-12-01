using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenIddict.EntityFrameworkCore.Models;
using SsoCore.Domain.Entities;
using SsoCore.Infrastructure.Data.Identity;

namespace SsoCore.Infrastructure.Data.Configurations
{
    public class ClientUserConfiguration : IEntityTypeConfiguration<ClientUser>
    {
        public void Configure(EntityTypeBuilder<ClientUser> builder)
        {
            builder.HasKey(x => new { x.ClientId, x.UserId });
            builder.Property(x => x.ClientId).IsRequired();
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            builder.ToTable("ClientUsers");

            builder.HasIndex(x => x.ClientId);
            builder.HasIndex(x => x.UserId);

            builder.HasOne<OpenIddictEntityFrameworkCoreApplication>()
           .WithMany()
           .HasForeignKey(x => x.ClientId)
           .HasPrincipalKey(a => a.Id)
           .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne<ApplicationUser>()
           .WithMany()
           .HasForeignKey(x => x.UserId)
           .HasPrincipalKey(a => a.Id)
           .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
