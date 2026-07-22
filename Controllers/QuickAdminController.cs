using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CVManagementSystem.Models.Domain;

namespace CVManagementSystem.Controllers;

[Authorize(Roles = "Admin")]
public class QuickAdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<QuickAdminController> _logger;

    public QuickAdminController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<QuickAdminController> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult MakeAdmin()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MakeAdmin(string email, string roleName)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            ViewBag.Error = "Please enter an email address.";
            return View();
        }

        if (string.IsNullOrWhiteSpace(roleName))
        {
            roleName = "Admin"; // Default to Admin
        }

        try
        {
            // Find user by email
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ViewBag.Error = $"No user found with email: {email}";
                return View();
            }

            // Check if role exists
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                ViewBag.Error = $"Role '{roleName}' does not exist.";
                return View();
            }

            // Get current roles
            var currentRoles = await _userManager.GetRolesAsync(user);

            // Remove all current roles
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            // Add new role
            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                ViewBag.Success = $"Successfully assigned '{roleName}' role to {user.Email}";
                _logger.LogInformation("User {Email} assigned to role {RoleName}", email, roleName);
            }
            else
            {
                ViewBag.Error = $"Failed to assign role: {string.Join(", ", result.Errors.Select(e => e.Description))}";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role to user {Email}", email);
            ViewBag.Error = "An error occurred. Please try again.";
        }

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ListUsers()
    {
        try
        {
            var users = _userManager.Users.ToList();
            var userList = new List<dynamic>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new
                {
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    Roles = string.Join(", ", roles),
                    user.IsBlocked
                });
            }

            ViewBag.Users = userList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing users");
            ViewBag.Error = "Error loading users.";
        }

        return View();
    }
}
