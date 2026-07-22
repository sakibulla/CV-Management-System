using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CVManagementSystem.Services;
using CVManagementSystem.Models.ViewModels;

namespace CVManagementSystem.Controllers;

[Authorize(Roles = "Recruiter,Admin")]
public class RecruiterController : Controller
{
    private readonly CVService _cvService;
    private readonly SearchService _searchService;
    private readonly ILogger<RecruiterController> _logger;

    public RecruiterController(
        CVService cvService,
        SearchService searchService,
        ILogger<RecruiterController> logger)
    {
        _cvService = cvService;
        _searchService = searchService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> CVs(string? search = null)
    {
        IEnumerable<CVManagementSystem.Models.Domain.CV> cvs;

        if (!string.IsNullOrWhiteSpace(search))
        {
            cvs = await _searchService.SearchCVsAsync(search);
        }
        else
        {
            cvs = await _cvService.GetAllCVsAsync();
        }

        var model = cvs.Select(cv => new RecruiterCVListViewModel
        {
            Id = cv.Id,
            Title = cv.Title,
            CandidateName = $"{cv.User?.FirstName} {cv.User?.LastName}".Trim(),
            CandidateEmail = cv.User?.Email ?? "N/A",
            PositionTitle = cv.Position?.Title ?? "N/A",
            PositionId = cv.PositionId,
            CreatedAt = cv.CreatedAt,
            UpdatedAt = cv.UpdatedAt
        }).ToList();

        ViewData["Search"] = search;
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> ViewCV(int id)
    {
        var cv = await _cvService.GetCVByIdAsync(id);
        if (cv == null)
            return NotFound();

        var model = new RecruiterCVDetailViewModel
        {
            Id = cv.Id,
            Title = cv.Title,
            CandidateName = $"{cv.User?.FirstName} {cv.User?.LastName}".Trim(),
            CandidateEmail = cv.User?.Email ?? "N/A",
            CandidateLocation = cv.User?.CandidateProfile?.Location ?? "Not specified",
            PositionId = cv.PositionId,
            PositionTitle = cv.Position?.Title ?? "N/A",
            PositionDescription = cv.Position?.Description ?? "N/A",
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

    [HttpGet]
    public async Task<IActionResult> PositionCVs(int positionId)
    {
        var cvs = await _cvService.GetCVsByPositionIdAsync(positionId);
        
        var model = cvs.Select(cv => new RecruiterCVListViewModel
        {
            Id = cv.Id,
            Title = cv.Title,
            CandidateName = $"{cv.User?.FirstName} {cv.User?.LastName}".Trim(),
            CandidateEmail = cv.User?.Email ?? "N/A",
            PositionTitle = cv.Position?.Title ?? "N/A",
            PositionId = cv.PositionId,
            CreatedAt = cv.CreatedAt,
            UpdatedAt = cv.UpdatedAt
        }).ToList();

        ViewData["PositionTitle"] = cvs.FirstOrDefault()?.Position?.Title ?? "Unknown Position";
        ViewData["PositionId"] = positionId;
        return View(model);
    }
}
