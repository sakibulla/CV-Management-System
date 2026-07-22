using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CVManagementSystem.Models.Domain;
using CVManagementSystem.Models.ViewModels;
using CVManagementSystem.Services;

namespace CVManagementSystem.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly AdminService _adminService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        AdminService adminService,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<AdminController> logger)
    {
        _adminService = adminService;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return RedirectToAction(nameof(UserManagement));
    }

    [HttpGet]
    public IActionResult Users()
    {
        return RedirectToAction(nameof(UserManagement));
    }

    [HttpGet]
    public async Task<IActionResult> UserManagement()
    {
        var users = _adminService.GetAllUsers();
        var model = new List<UserListViewModel>();

        foreach (var user in users)
        {
            var role = await _adminService.GetUserRoleAsync(user.Id);
            model.Add(new UserListViewModel
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = role ?? "No Role",
                IsBlocked = user.IsBlocked
            });
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> UserDetails(string id)
    {
        if (string.IsNullOrEmpty(id))
            return BadRequest();

        var user = await _adminService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();

        var role = await _adminService.GetUserRoleAsync(id);
        var availableRoles = _roleManager.Roles.Select(r => r.Name!).ToList();

        var model = new UserDetailViewModel
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CurrentRole = role ?? "Unknown",
            IsBlocked = user.IsBlocked,
            AvailableRoles = availableRoles
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignRole(string userId, string roleName)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleName))
            return BadRequest();

        try
        {
            var success = await _adminService.AssignRoleAsync(userId, roleName);
            if (success)
            {
                TempData["Success"] = $"Role '{roleName}' assigned successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to assign role.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role {RoleName} to user {UserId}", roleName, userId);
            TempData["Error"] = "An error occurred while assigning the role.";
        }

        return RedirectToAction(nameof(UserDetails), new { id = userId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BlockUser(string id)
    {
        if (string.IsNullOrEmpty(id))
            return BadRequest();

        try
        {
            var success = await _adminService.BlockUserAsync(id);
            if (success)
            {
                TempData["Success"] = "User blocked successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to block user.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error blocking user {UserId}", id);
            TempData["Error"] = "An error occurred while blocking the user.";
        }

        return RedirectToAction(nameof(UserDetails), new { id = id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UnblockUser(string id)
    {
        if (string.IsNullOrEmpty(id))
            return BadRequest();

        try
        {
            var success = await _adminService.UnblockUserAsync(id);
            if (success)
            {
                TempData["Success"] = "User unblocked successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to unblock user.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unblocking user {UserId}", id);
            TempData["Error"] = "An error occurred while unblocking the user.";
        }

        return RedirectToAction(nameof(UserDetails), new { id = id });
    }

    [HttpGet]
    public async Task<IActionResult> EditProfile(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return BadRequest();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        var profile = await _adminService.GetOrCreateCandidateProfileAsync(userId);
        var attributes = await _adminService.GetAllAttributesAsync();

        var model = new AdminEditProfileViewModel
        {
            UserId = userId,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Location = profile.Location,
            ProfileAttributes = attributes.Select(a => new AttributeDisplayViewModel
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Type,
                CurrentValue = profile.ProfileAttributes
                    .FirstOrDefault(pa => pa.PositionAttributeId == a.Id)?.Value
            }).ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(string userId, AdminEditProfileViewModel model)
    {
        if (string.IsNullOrEmpty(userId))
            return BadRequest();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        try
        {
            // Update user basic info
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            await _userManager.UpdateAsync(user);

            // Update profile
            await _adminService.UpdateCandidateProfileAsync(userId, model.Location);

            // Update attributes
            var attributeValues = model.AttributeValues ?? new Dictionary<int, string>();
            await _adminService.UpdateProfileAttributesAsync(userId, attributeValues);

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction(nameof(UserDetails), new { id = userId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile for user {UserId}", userId);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the profile.");
            return await EditProfile(userId);
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditCV(int cvId)
    {
        var cv = await _adminService.GetCVByIdAsync(cvId);
        if (cv == null)
            return NotFound();

        var projects = await _adminService.GetProjectsByUserIdAsync(cv.UserId);
        var selectedProjectIds = cv.CVProjects.Select(cp => cp.ProjectId).ToArray();

        var model = new AdminEditCVViewModel
        {
            Id = cv.Id,
            PositionId = cv.PositionId,
            Title = cv.Title,
            PositionTitle = cv.Position?.Title ?? "N/A",
            CandidateName = $"{cv.User?.FirstName} {cv.User?.LastName}".Trim(),
            Attributes = cv.Position?.Attributes.Select(a => new CVAttributeViewModel
            {
                PositionAttributeId = a.Id,
                AttributeName = a.Name,
                CurrentValue = cv.CVAttributes
                    .FirstOrDefault(ca => ca.PositionAttributeId == a.Id)?.Value
            }).ToList() ?? new List<CVAttributeViewModel>(),
            AvailableProjects = projects.Select(p => new ProjectSelectionViewModel
            {
                Id = p.Id,
                Name = p.Name,
                IsSelected = selectedProjectIds.Contains(p.Id)
            }).ToList(),
            SelectedProjectIds = selectedProjectIds
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCV(int cvId, AdminEditCVViewModel model)
    {
        var cv = await _adminService.GetCVByIdAsync(cvId);
        if (cv == null)
            return NotFound();

        if (!ModelState.IsValid)
            return await EditCV(cvId);

        try
        {
            var attributeValues = model.AttributeValues ?? new Dictionary<int, string>();
            var projectIds = model.SelectedProjectIds ?? Array.Empty<int>();

            await _adminService.UpdateCVAsync(cvId, model.Title, attributeValues, projectIds);
            TempData["Success"] = "CV updated successfully!";
            return RedirectToAction("ViewCV", "Recruiter", new { id = cvId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating CV {CVId}", cvId);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the CV.");
            return await EditCV(cvId);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveRole(string userId, string roleName)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleName))
            return BadRequest();

        try
        {
            var success = await _adminService.RemoveRoleAsync(userId, roleName);
            if (success)
            {
                TempData["Success"] = $"Role '{roleName}' removed successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to remove role.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role {RoleName} from user {UserId}", roleName, userId);
            TempData["Error"] = "An error occurred while removing the role.";
        }

        return RedirectToAction(nameof(UserDetails), new { id = userId });
    }
}
