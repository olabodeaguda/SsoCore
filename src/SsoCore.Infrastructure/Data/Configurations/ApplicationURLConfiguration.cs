using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenIddict.EntityFrameworkCore.Models;
using SsoCore.Domain.Entities;

namespace SsoCore.Infrastructure.Data.Configurations
{
    public class ApplicationURLConfiguration : IEntityTypeConfiguration<ApplicationURL>
    {
        public void Configure(EntityTypeBuilder<ApplicationURL> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.URL).IsRequired();
            builder.Property(x => x.ApplicationId).IsRequired();

            builder.HasOne<OpenIddictEntityFrameworkCoreApplication>()
            .WithMany() 
            .HasForeignKey(x => x.ApplicationId)
            .HasPrincipalKey(a => a.Id)
            .OnDelete(DeleteBehavior.NoAction);

            builder.ToTable("ApplicationURLs");
        }
    }
}
