using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CVManagementSystem.Models.Domain;
using CVManagementSystem.Models.ViewModels;
using CVManagementSystem.Services;

namespace CVManagementSystem.Controllers;

[Authorize]
public class SearchController : Controller
{
    private readonly SearchService _searchService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<SearchController> _logger;

    public SearchController(
        SearchService searchService,
        UserManager<ApplicationUser> userManager,
        ILogger<SearchController> logger)
    {
        _searchService = searchService;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return View(new SearchResultsViewModel { Query = string.Empty });

        try
        {
            var positions = await _searchService.SearchPositionsAsync(query);
            var cvs = await _searchService.SearchCVsAsync(query);

            var model = new SearchResultsViewModel
            {
                Query = query,
                Positions = positions.Select(p => new SearchPositionResultViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    CreatedByName = p.CreatedByUser != null 
                        ? $"{p.CreatedByUser.FirstName} {p.CreatedByUser.LastName}".Trim()
                        : "Unknown",
                    CreatedAt = p.CreatedAt
                }).ToList(),
                CVs = cvs.Select(cv => new SearchCVResultViewModel
                {
                    Id = cv.Id,
                    Title = cv.Title,
                    PositionTitle = cv.Position?.Title ?? "N/A",
                    CandidateName = cv.User != null
                        ? $"{cv.User.FirstName} {cv.User.LastName}".Trim()
                        : "Unknown",
                    CreatedAt = cv.CreatedAt
                }).ToList()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching with query: {Query}", query);
            ModelState.AddModelError(string.Empty, "An error occurred during search.");
            return View(new SearchResultsViewModel { Query = query });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return RedirectToAction(nameof(Index));

        return RedirectToAction(nameof(Index), new { query = query.Trim() });
    }
}
