using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mimre.Domain.Entities;

namespace Mimre.Infrastructure.Persistence.Configurations;

public class GalleryConfiguration : IEntityTypeConfiguration<Gallery>
{
    public void Configure(EntityTypeBuilder<Gallery> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(g => g.Slug)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(g => new { g.UserId, g.Slug })
            .IsUnique();

        builder.Property(g => g.PasswordHash)
            .HasMaxLength(512);

        builder.HasMany(g => g.Albums)
            .WithOne(a => a.Gallery)
            .HasForeignKey(a => a.GalleryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(g => g.ShareLinks)
            .WithOne(s => s.Gallery)
            .HasForeignKey(s => s.GalleryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
