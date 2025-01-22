using Xunit;
using LibraryManagementDomain.Entities;
//using LibraryManagementAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementTests
{ 
public class RoleManagementTests
{
    [Fact]
    public async Task AddRole_ShouldAddRoleSuccessfully()
    {
        // Arrange
        var dbContext = MockLibraryDBContext.GetDbContext();
        var role = new Role { Name = "Editor" };

        // Act
        dbContext.Roles.Add(role);
        await dbContext.SaveChangesAsync();

        // Assert
        var addedRole = await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "Editor");
        Assert.NotNull(addedRole);
        Assert.Equal("Editor", addedRole.Name);
    }

    [Fact]
    public async Task GetRoles_ShouldReturnAllRoles()
    {
        // Arrange
        var dbContext = MockLibraryDBContext.GetDbContext();
        dbContext.Roles.Add(new Role { Name = "Admin" });
        dbContext.Roles.Add(new Role { Name = "Viewer" });
        await dbContext.SaveChangesAsync();

        // Act
        var roles = await dbContext.Roles.ToListAsync();

        // Assert
        Assert.Equal(2, roles.Count);
        Assert.Contains(roles, r => r.Name == "Admin");
        Assert.Contains(roles, r => r.Name == "Viewer");
    }

    [Fact]
    public async Task AddPermission_ShouldAddPermissionSuccessfully()
    {
        // Arrange
        var dbContext = MockLibraryDBContext.GetDbContext();
        var role = new Role { Name = "Admin" };
        dbContext.Roles.Add(role);
        await dbContext.SaveChangesAsync();

        var permission = new RolePermission
        {
            RoleId = role.Id,
            Permission = "ViewAuthors"
        };

        // Act
        dbContext.RolePermissions.Add(permission);
        await dbContext.SaveChangesAsync();

        // Assert
        var addedPermission = await dbContext.RolePermissions
            .FirstOrDefaultAsync(p => p.Permission == "ViewAuthors");
        Assert.NotNull(addedPermission);
        Assert.Equal("ViewAuthors", addedPermission.Permission);
        Assert.Equal(role.Id, addedPermission.RoleId);
    }

    [Fact]
    public async Task GetPermissions_ShouldReturnPermissionsForRole()
    {
        // Arrange
        var dbContext = MockLibraryDBContext.GetDbContext();
        var role = new Role { Name = "Admin" };
        dbContext.Roles.Add(role);
        await dbContext.SaveChangesAsync();

        dbContext.RolePermissions.Add(new RolePermission { RoleId = role.Id, Permission = "ViewAuthors" });
        dbContext.RolePermissions.Add(new RolePermission { RoleId = role.Id, Permission = "EditAuthors" });
        await dbContext.SaveChangesAsync();

        // Act
        var permissions = await dbContext.RolePermissions
            .Where(p => p.RoleId == role.Id)
            .Select(p => p.Permission)
            .ToListAsync();

        // Assert
        Assert.Equal(2, permissions.Count);
        Assert.Contains(permissions, p => p == "ViewAuthors");
        Assert.Contains(permissions, p => p == "EditAuthors");
    }
}
}

