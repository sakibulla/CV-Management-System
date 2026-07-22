using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CVManagementSystem.Models.Domain;
using CVManagementSystem.Models.ViewModels;
using CVManagementSystem.Services;

namespace CVManagementSystem.Controllers;

[Authorize(Roles = "Candidate")]
public class CVsController : Controller
{
    private readonly CVService _cvService;
    private readonly ProjectService _projectService;
    private readonly PositionService _positionService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<CVsController> _logger;

    public CVsController(
        CVService cvService,
        ProjectService projectService,
        PositionService positionService,
        UserManager<ApplicationUser> userManager,
        ILogger<CVsController> logger)
    {
        _cvService = cvService;
        _projectService = projectService;
        _positionService = positionService;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var cvs = await _cvService.GetCVsByUserIdAsync(user.Id);
        var model = cvs.Select(cv => new CVListViewModel
        {
            Id = cv.Id,
            Title = cv.Title,
            PositionTitle = cv.Position?.Title ?? "N/A",
            PositionId = cv.PositionId,
            CreatedAt = cv.CreatedAt,
            UpdatedAt = cv.UpdatedAt
        }).ToList();

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int positionId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        // Check if user already has a CV for this position
        var existingCV = await _cvService.GetCVByPositionAndUserAsync(positionId, user.Id);
        if (existingCV != null)
        {
            TempData["Info"] = "You already have a CV for this position. Redirecting to edit.";
            return RedirectToAction("Edit", new { id = existingCV.Id });
        }

        var position = await _positionService.GetPositionByIdAsync(positionId);
        if (position == null)
            return NotFound("Position not found");

        var projects = await _projectService.GetProjectsByUserIdAsync(user.Id);
        var attributes = position.Attributes;

        var model = new CVFormViewModel
        {
            PositionId = positionId,
            PositionTitle = position.Title,
            Attributes = attributes.Select(a => new CVAttributeViewModel
            {
                PositionAttributeId = a.Id,
                AttributeName = a.Name
            }).ToList(),
            AvailableProjects = projects.Select(p => new ProjectSelectionViewModel
            {
                Id = p.Id,
                Name = p.Name,
                IsSelected = false
            }).ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int positionId, CVFormViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        // Check for duplicate CV
        var existingCV = await _cvService.GetCVByPositionAndUserAsync(positionId, user.Id);
        if (existingCV != null)
        {
            TempData["Info"] = "You already have a CV for this position. Redirecting to edit.";
            return RedirectToAction("Edit", new { id = existingCV.Id });
        }

        var position = await _positionService.GetPositionByIdAsync(positionId);
        if (position == null)
            return NotFound("Position not found");

        if (!ModelState.IsValid)
        {
            // Reload form data
            var projects = await _projectService.GetProjectsByUserIdAsync(user.Id);
            model.Attributes = position.Attributes.Select(a => new CVAttributeViewModel
            {
                PositionAttributeId = a.Id,
                AttributeName = a.Name,
                CurrentValue = model.AttributeValues?.ContainsKey(a.Id) == true 
                    ? model.AttributeValues[a.Id] 
                    : null
            }).ToList();
            model.AvailableProjects = projects.Select(p => new ProjectSelectionViewModel
            {
                Id = p.Id,
                Name = p.Name,
                IsSelected = model.SelectedProjectIds?.Contains(p.Id) ?? false
            }).ToList();
            return View(model);
        }

        try
        {
            var attributeValues = model.AttributeValues ?? new Dictionary<int, string>();
            var projectIds = model.SelectedProjectIds ?? Array.Empty<int>();

            await _cvService.CreateCVAsync(positionId, user.Id, model.Title, attributeValues, projectIds);
            TempData["Success"] = "CV created successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating CV for user {UserId}", user.Id);
            ModelState.AddModelError(string.Empty, "An error occurred while creating the CV.");
            return await Create(positionId);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var cv = await _cvService.GetCVByIdAsync(id);
        if (cv == null)
            return NotFound();

        if (cv.UserId != user.Id)
            return Forbid();

        var projects = await _projectService.GetProjectsByUserIdAsync(user.Id);
        var selectedProjectIds = cv.CVProjects.Select(cp => cp.ProjectId).ToArray();

        var model = new CVFormViewModel
        {
            Id = cv.Id,
            PositionId = cv.PositionId,
            Title = cv.Title,
            PositionTitle = cv.Position?.Title ?? "N/A",
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
    public async Task<IActionResult> Edit(int id, CVFormViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var cv = await _cvService.GetCVByIdAsync(id);
        if (cv == null)
            return NotFound();

        if (cv.UserId != user.Id)
            return Forbid();

        if (!ModelState.IsValid)
            return await Edit(id);

        try
        {
            var attributeValues = model.AttributeValues ?? new Dictionary<int, string>();
            var projectIds = model.SelectedProjectIds ?? Array.Empty<int>();

            await _cvService.UpdateCVAsync(id, model.Title, attributeValues, projectIds);
            TempData["Success"] = "CV updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating CV {CVId}", id);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the CV.");
            return await Edit(id);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var cv = await _cvService.GetCVByIdAsync(id);
        if (cv == null)
            return NotFound();

        if (cv.UserId != user.Id)
            return Forbid();

        try
        {
            await _cvService.DeleteCVAsync(id);
            TempData["Success"] = "CV deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting CV {CVId}", id);
            TempData["Error"] = "An error occurred while deleting the CV.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var cv = await _cvService.GetCVByIdAsync(id);
        if (cv == null)
            return NotFound();

        var model = new CVDetailViewModel
        {
            Id = cv.Id,
            Title = cv.Title,
            PositionId = cv.PositionId,
            PositionTitle = cv.Position?.Title ?? "N/A",
            CreatedAt = cv.CreatedAt,
            UpdatedAt = cv.UpdatedAt,
            Attributes = cv.CVAttributes.Select(ca => new CVAttributeDetailViewModel
            {
                AttributeName = ca.PositionAttribute?.Name ?? "N/A",
                Value = ca.Value
            }).ToList(),
            Projects = cv.CVProjects.Select(cp => new ProjectDetailViewModel
            {
                Id = cp.Project!.Id,
                Name = cp.Project.Name,
                Description = cp.Project.Description,
                Technologies = cp.Project.Technologies
            }).ToList()
        };

        return View(model);
    }
}
