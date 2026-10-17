using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LondonEstate.Core.Data
{
    /// <summary>
    /// Factory for creating DbContext instances at design time.
    /// This is used by Entity Framework Core tooling (migrations, scaffolding, etc.)
    /// </summary>
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // Try to get connection string from configuration
            // The appsettings.json is in the LondonEstate.Api project, so we need to navigate there
            var basePath = Directory.GetCurrentDirectory();

            // If we're in the Core project directory, go up one level to solution root
            if (basePath.EndsWith("LondonEstate.Core"))
            {
                basePath = Directory.GetParent(basePath)?.FullName ?? basePath;
            }

            // Look for Api folder
            var apiPath = Path.Combine(basePath, "LondonEstate.Api");
            if (!Directory.Exists(apiPath))
            {
                // If Api folder not found, try current directory
                apiPath = basePath;
            }

            var configuration = new ConfigurationBuilder()
                .SetBasePath(apiPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' not found in configuration at: " + 
                    Path.Combine(apiPath, "appsettings.json"));

            optionsBuilder.UseSqlServer(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
