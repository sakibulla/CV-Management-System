using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CVManagementSystem.Models.Domain;
using CVManagementSystem.Models.ViewModels;
using CVManagementSystem.Services;

namespace CVManagementSystem.Controllers;

[Authorize]
public class CandidateProfileController : Controller
{
    private readonly CandidateProfileService _profileService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<CandidateProfileController> _logger;

    public CandidateProfileController(
        CandidateProfileService profileService,
        UserManager<ApplicationUser> userManager,
        ILogger<CandidateProfileController> logger)
    {
        _profileService = profileService;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return RedirectToAction(nameof(Me));
    }

    [HttpGet]
    public async Task<IActionResult> Me()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var profile = await _profileService.GetOrCreateProfileAsync(user.Id);
        var attributes = await _profileService.GetAllAttributesAsync();

        var model = new CandidateProfileViewModel
        {
            ProfileId = profile.Id,
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            Location = profile.Location,
            UpdatedAt = profile.UpdatedAt,
            AvailableAttributes = attributes.Select(a => new AttributeDisplayViewModel
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
    public async Task<IActionResult> UpdateInfo(CandidateProfileViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        try
        {
            await _profileService.UpdateProfileAsync(user.Id, model.FirstName, model.LastName, model.Location);

            var attributeValues = model.AttributeValues ?? new Dictionary<int, string>();
            await _profileService.UpdateProfileAttributesAsync(user.Id, attributeValues);

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Me));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile for user {UserId}", user.Id);
            ModelState.AddModelError(string.Empty, "An error occurred while updating your profile.");
            return await Me();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Info()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var profile = await _profileService.GetProfileWithAttributesAsync(user.Id);
        if (profile == null)
            return NotFound();

        var attributes = await _profileService.GetAllAttributesAsync();

        var model = new CandidateProfileViewModel
        {
            ProfileId = profile.Id,
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            Location = profile.Location,
            UpdatedAt = profile.UpdatedAt,
            AvailableAttributes = attributes.Select(a => new AttributeDisplayViewModel
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
}
