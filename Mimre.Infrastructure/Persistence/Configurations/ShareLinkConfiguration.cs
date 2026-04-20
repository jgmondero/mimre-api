using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mimre.Domain.Entities;

namespace Mimre.Infrastructure.Persistence.Configurations;

public class ShareLinkConfiguration : IEntityTypeConfiguration<ShareLink>
{
    public void Configure(EntityTypeBuilder<ShareLink> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Token)
            .IsRequired()
            .HasMaxLength(64);

        builder.HasIndex(s => s.Token)
            .IsUnique();

        builder.Property(s => s.ClientEmail)
            .HasMaxLength(256);

        builder.Property(s => s.DownloadPermission)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasMany(s => s.DownloadLogs)
            .WithOne(d => d.ShareLink)
            .HasForeignKey(d => d.ShareLinkId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
