using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Mimre.Infrastructure.Persistence;

public class MimreDbContextFactory : IDesignTimeDbContextFactory<MimreDbContext>
{
    public MimreDbContext CreateDbContext(string[] args)
    {
        // Walk up from Infrastructure project to find appsettings.json in Api project
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../Mimre.Api");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<MimreDbContext>();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

        return new MimreDbContext(optionsBuilder.Options);
    }
}
