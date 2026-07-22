using Microsoft.EntityFrameworkCore;
using CVManagementSystem.Data;
using CVManagementSystem.Models.Domain;

namespace CVManagementSystem.Services;

public class SearchService
{
    private readonly AppDbContext _context;

    public SearchService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Position>> SearchPositionsAsync(string query)
    {
        var searchTerm = query.ToLower();
        return await _context.Positions
            .Where(p => p.Title.ToLower().Contains(searchTerm) || 
                       p.Description.ToLower().Contains(searchTerm))
            .Include(p => p.CreatedByUser)
            .ToListAsync();
    }

    public async Task<IEnumerable<CV>> SearchCVsAsync(string query, string? userId = null)
    {
        var searchTerm = query.ToLower();
        var result = _context.CVs
            .Include(cv => cv.Position)
            .Where(cv => cv.Title.ToLower().Contains(searchTerm) || 
                        cv.Position!.Title.ToLower().Contains(searchTerm));

        if (!string.IsNullOrEmpty(userId))
        {
            result = result.Where(cv => cv.UserId == userId);
        }

        return await result.ToListAsync();
    }
}
