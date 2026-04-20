using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mimre.Domain.Entities;

namespace Mimre.Infrastructure.Persistence.Configurations;

public class DownloadLogConfiguration : IEntityTypeConfiguration<DownloadLog>
{
    public void Configure(EntityTypeBuilder<DownloadLog> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.IpAddress)
            .IsRequired()
            .HasMaxLength(45); // supports IPv6
    }
}
