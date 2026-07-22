using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CVManagementSystem.Models.Domain;
using CVManagementSystem.Models.ViewModels;
using CVManagementSystem.Services;

namespace CVManagementSystem.Controllers;

[Authorize(Roles = "Candidate")]
public class ProjectsController : Controller
{
    private readonly ProjectService _projectService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(
        ProjectService projectService,
        UserManager<ApplicationUser> userManager,
        ILogger<ProjectsController> logger)
    {
        _projectService = projectService;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var projects = await _projectService.GetProjectsByUserIdAsync(user.Id);
        var model = projects.Select(p => new ProjectViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Technologies = p.Technologies,
            CreatedAt = p.CreatedAt
        }).ToList();

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new ProjectFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectFormViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _projectService.CreateProjectAsync(user.Id, model.Name, model.Description, model.Technologies);
            TempData["Success"] = "Project created successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating project for user {UserId}", user.Id);
            ModelState.AddModelError(string.Empty, "An error occurred while creating the project.");
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var project = await _projectService.GetProjectByIdAsync(id);
        if (project == null)
            return NotFound();

        if (project.UserId != user.Id)
            return Forbid();

        var model = new ProjectFormViewModel
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Technologies = project.Technologies
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProjectFormViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var project = await _projectService.GetProjectByIdAsync(id);
        if (project == null)
            return NotFound();

        if (project.UserId != user.Id)
            return Forbid();

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _projectService.UpdateProjectAsync(id, model.Name, model.Description, model.Technologies);
            TempData["Success"] = "Project updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating project {ProjectId}", id);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the project.");
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var project = await _projectService.GetProjectByIdAsync(id);
        if (project == null)
            return NotFound();

        if (project.UserId != user.Id)
            return Forbid();

        try
        {
            await _projectService.DeleteProjectAsync(id);
            TempData["Success"] = "Project deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting project {ProjectId}", id);
            TempData["Error"] = "An error occurred while deleting the project.";
            return RedirectToAction(nameof(Index));
        }
    }
}
