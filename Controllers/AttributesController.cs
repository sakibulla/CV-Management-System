using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CVManagementSystem.Services;

namespace CVManagementSystem.Controllers;

[Authorize(Roles = "Recruiter,Admin")]
public class AttributesController : Controller
{
    private readonly AttributeService _attributeService;

    public AttributesController(AttributeService attributeService)
    {
        _attributeService = attributeService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var attributes = _attributeService.GetAllAttributes();
        
        if (!User.IsInRole("Admin"))
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            attributes = attributes.Where(a => a.CreatedBy == userId);
        }

        return View(await attributes.ToListAsync());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string name, string type, string dropdownOptions)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(type))
        {
            ModelState.AddModelError(string.Empty, "Name and Type are required");
            return View();
        }

        if (type == "Dropdown" && string.IsNullOrWhiteSpace(dropdownOptions))
        {
            ModelState.AddModelError(string.Empty, "Dropdown options are required");
            return View();
        }

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        await _attributeService.CreateAttributeAsync(name, type, dropdownOptions ?? string.Empty, userId!);
        
        TempData["Success"] = "Attribute created successfully";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var attribute = await _attributeService.GetAttributeByIdAsync(id);
        if (attribute == null)
            return NotFound();

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!User.IsInRole("Admin") && attribute.CreatedBy != userId)
            return Forbid();

        return View(attribute);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, string name, string type, string dropdownOptions)
    {
        var attribute = await _attributeService.GetAttributeByIdAsync(id);
        if (attribute == null)
            return NotFound();

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!User.IsInRole("Admin") && attribute.CreatedBy != userId)
            return Forbid();

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(type))
        {
            ModelState.AddModelError(string.Empty, "Name and Type are required");
            return View(attribute);
        }

        await _attributeService.UpdateAttributeAsync(id, name, type, dropdownOptions ?? string.Empty);
        TempData["Success"] = "Attribute updated successfully";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var attribute = await _attributeService.GetAttributeByIdAsync(id);
        if (attribute == null)
            return NotFound();

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!User.IsInRole("Admin") && attribute.CreatedBy != userId)
            return Forbid();

        var inUse = await _attributeService.IsAttributeInUseAsync(id);
        if (inUse)
        {
            TempData["Error"] = "Cannot delete attribute in use";
            return RedirectToAction(nameof(Index));
        }

        await _attributeService.DeleteAttributeAsync(id);
        TempData["Success"] = "Attribute deleted successfully";
        return RedirectToAction(nameof(Index));
    }
}
