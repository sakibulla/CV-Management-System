using Microsoft.AspNetCore.Identity;
using CVManagementSystem.Models.Domain;

namespace CVManagementSystem.Data;

public class SeedData
{
    public static async Task InitializeAsync(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Seed roles
        string[] roleNames = { "Admin", "Recruiter", "Candidate" };
        IdentityResult roleResult;

        foreach (var roleName in roleNames)
        {
            var roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Create default admin user (optional)
        var adminUser = await userManager.FindByEmailAsync("admin@example.com");
        if (adminUser == null)
        {
            var newAdminUser = new ApplicationUser
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true
            };

            string adminPassword = "AdminPassword123!";
            var createAdminResult = await userManager.CreateAsync(newAdminUser, adminPassword);

            if (createAdminResult.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdminUser, "Admin");
            }
        }

        // Assign admin role to specific user
        var specificUser = await userManager.FindByIdAsync("7e2424d4-f789-4186-8cf3-dad28626b4cd");
        if (specificUser != null)
        {
            var userRoles = await userManager.GetRolesAsync(specificUser);
            if (!userRoles.Contains("Admin"))
            {
                if (userRoles.Any())
                    await userManager.RemoveFromRolesAsync(specificUser, userRoles);
                await userManager.AddToRoleAsync(specificUser, "Admin");
            }
        }

        await context.SaveChangesAsync();
    }
}
