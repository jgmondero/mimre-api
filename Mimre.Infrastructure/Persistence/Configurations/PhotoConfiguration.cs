using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mimre.Domain.Entities;

namespace Mimre.Infrastructure.Persistence.Configurations;

public class PhotoConfiguration : IEntityTypeConfiguration<Photo>
{
    public void Configure(EntityTypeBuilder<Photo> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.OriginalFileName)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(p => p.BlobPath)
            .IsRequired()
            .HasMaxLength(1024);

        builder.Property(p => p.ThumbnailBlobPath)
            .HasMaxLength(1024);

        builder.Property(p => p.WatermarkedBlobPath)
            .HasMaxLength(1024);

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(p => p.MetadataJson)
            .HasColumnType("nvarchar(max)");
    }
}
