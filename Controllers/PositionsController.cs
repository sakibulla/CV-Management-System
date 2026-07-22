using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CVManagementSystem.Services;

namespace CVManagementSystem.Controllers;

[Authorize] // Changed from specific roles to just authenticated
public class PositionsController : Controller
{
    private readonly PositionService _positionService;
    private readonly AttributeService _attributeService;

    public PositionsController(PositionService positionService, AttributeService attributeService)
    {
        _positionService = positionService;
        _attributeService = attributeService;
    }

    [HttpGet]
    public async Task<IActionResult> Browse()
    {
        // This action is for all authenticated users to view available positions
        var positions = await _positionService.GetAllPositions().ToListAsync();
        return View("Browse", positions);
    }

    [HttpGet]
    [Authorize(Roles = "Recruiter,Admin")] // Only recruiters and admins can see their positions list
    public async Task<IActionResult> Index()
    {
        var positions = _positionService.GetAllPositions();
        
        if (!User.IsInRole("Admin"))
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            positions = positions.Where(p => p.CreatedBy == userId);
        }

        return View(await positions.ToListAsync());
    }

    [HttpGet]
    [Authorize(Roles = "Recruiter,Admin")]
    public async Task<IActionResult> Create()
    {
        var attributes = await _attributeService.GetAllAttributes().ToListAsync();
        ViewData["Attributes"] = new MultiSelectList(attributes, "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Recruiter,Admin")]
    public async Task<IActionResult> Create(string title, string description, int[] selectedAttributeIds)
    {
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description) || selectedAttributeIds.Length == 0)
        {
            ModelState.AddModelError(string.Empty, "Title, Description, and at least one Attribute are required");
            var attributes = await _attributeService.GetAllAttributes().ToListAsync();
            ViewData["Attributes"] = new MultiSelectList(attributes, "Id", "Name", selectedAttributeIds);
            return View();
        }

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        await _positionService.CreatePositionAsync(title, description, userId!, selectedAttributeIds);
        
        TempData["Success"] = "Position created successfully";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var position = await _positionService.GetPositionByIdAsync(id);
        if (position == null)
            return NotFound();

        return View(position);
    }

    [HttpGet]
    [Authorize(Roles = "Recruiter,Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var position = await _positionService.GetPositionByIdAsync(id);
        if (position == null)
            return NotFound();

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!User.IsInRole("Admin") && position.CreatedBy != userId)
            return Forbid();

        var attributes = await _attributeService.GetAllAttributes().ToListAsync();
        var selectedAttributeIds = position.Attributes.Select(a => a.Id).ToArray();
        ViewData["Attributes"] = new MultiSelectList(attributes, "Id", "Name", selectedAttributeIds);
        ViewData["PositionId"] = position.Id;
        
        return View(position);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Recruiter,Admin")]
    public async Task<IActionResult> Edit(int id, string title, string description, int[] selectedAttributeIds)
    {
        var position = await _positionService.GetPositionByIdAsync(id);
        if (position == null)
            return NotFound();

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!User.IsInRole("Admin") && position.CreatedBy != userId)
            return Forbid();

        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description) || selectedAttributeIds.Length == 0)
        {
            ModelState.AddModelError(string.Empty, "Title, Description, and at least one Attribute are required");
            var attributes = await _attributeService.GetAllAttributes().ToListAsync();
            ViewData["Attributes"] = new MultiSelectList(attributes, "Id", "Name", selectedAttributeIds);
            ViewData["PositionId"] = id;
            var updatedPosition = new { Title = title, Description = description, SelectedAttributeIds = selectedAttributeIds };
            return View(updatedPosition);
        }

        await _positionService.UpdatePositionAsync(id, title, description, selectedAttributeIds);
        TempData["Success"] = "Position updated successfully";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Recruiter,Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var position = await _positionService.GetPositionByIdAsync(id);
        if (position == null)
            return NotFound();

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!User.IsInRole("Admin") && position.CreatedBy != userId)
            return Forbid();

        await _positionService.DeletePositionAsync(id);
        TempData["Success"] = "Position deleted successfully";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Recruiter,Admin")]
    public async Task<IActionResult> Duplicate(int id)
    {
        var position = await _positionService.GetPositionByIdAsync(id);
        if (position == null)
            return NotFound();

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!User.IsInRole("Admin") && position.CreatedBy != userId)
            return Forbid();

        await _positionService.DuplicatePositionAsync(id, userId!);
        TempData["Success"] = "Position duplicated successfully";
        return RedirectToAction(nameof(Index));
    }
}
