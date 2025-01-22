using Microsoft.EntityFrameworkCore;
using LibraryManagementInfrastructure;
//using LibraryManagementAPI.Data;

public static class MockLibraryDBContext
{
    public static LibraryDBContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<LibraryDBContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        var dbContext = new LibraryDBContext(options);
        dbContext.Database.EnsureDeleted(); // Clears the database before each test
        dbContext.Database.EnsureCreated(); // Recreates the database schema

        return dbContext;
    }
}
