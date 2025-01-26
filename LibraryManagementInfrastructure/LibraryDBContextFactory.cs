using LibraryManagementInfrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

public class LibraryDBContextFactory : IDesignTimeDbContextFactory<LibraryDBContext>
{
    public LibraryDBContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<LibraryDBContext>();
        var databaseProvider = configuration["DatabaseProvider"];
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (databaseProvider == "PostgreSQL")
        {
            optionsBuilder.UseNpgsql(connectionString);
        }
        else if (databaseProvider == "SqlServer")
        {
            optionsBuilder.UseSqlServer(connectionString);
        }

        return new LibraryDBContext(optionsBuilder.Options);
    }
}
