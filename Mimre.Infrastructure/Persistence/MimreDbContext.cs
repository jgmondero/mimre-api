using Microsoft.EntityFrameworkCore;
using Mimre.Domain.Common;
using Mimre.Domain.Entities;

namespace Mimre.Infrastructure.Persistence;

public class MimreDbContext(DbContextOptions<MimreDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Gallery> Galleries => Set<Gallery>();
    public DbSet<Album> Albums => Set<Album>();
    public DbSet<Photo> Photos => Set<Photo>();
    public DbSet<ShareLink> ShareLinks => Set<ShareLink>();
    public DbSet<DownloadLog> DownloadLogs => Set<DownloadLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Applies all IEntityTypeConfiguration classes in this assembly automatically
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MimreDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        // Auto-set CreatedAt and UpdatedAt on all AuditableEntities
        var entries = ChangeTracker.Entries<AuditableEntity>();
        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedOn = now;

            if (entry.State is EntityState.Added or EntityState.Modified)
                entry.Entity.ModifiedOn = now;
        }

        return await base.SaveChangesAsync(ct);
    }
}
