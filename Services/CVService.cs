using Microsoft.EntityFrameworkCore;
using CVManagementSystem.Data;
using CVManagementSystem.Models.Domain;

namespace CVManagementSystem.Services;

public class CVService
{
    private readonly AppDbContext _context;

    public CVService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CV>> GetCVsByUserIdAsync(string userId)
    {
        return await _context.CVs
            .Where(cv => cv.UserId == userId)
            .Include(cv => cv.Position)
            .OrderByDescending(cv => cv.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<CV>> GetAllCVsAsync()
    {
        return await _context.CVs
            .Include(cv => cv.Position)
            .Include(cv => cv.User)
            .OrderByDescending(cv => cv.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<CV>> GetCVsByPositionIdAsync(int positionId)
    {
        return await _context.CVs
            .Where(cv => cv.PositionId == positionId)
            .Include(cv => cv.Position)
            .Include(cv => cv.User)
            .OrderByDescending(cv => cv.CreatedAt)
            .ToListAsync();
    }

    public async Task<CV?> GetCVByIdAsync(int id)
    {
        return await _context.CVs
            .Include(cv => cv.Position)
            .ThenInclude(p => p!.Attributes)
            .Include(cv => cv.User)
            .Include(cv => cv.CVAttributes)
            .ThenInclude(ca => ca.PositionAttribute)
            .Include(cv => cv.CVProjects)
            .ThenInclude(cp => cp.Project)
            .FirstOrDefaultAsync(cv => cv.Id == id);
    }

    public async Task<CV?> GetCVByPositionAndUserAsync(int positionId, string userId)
    {
        return await _context.CVs
            .FirstOrDefaultAsync(cv => cv.PositionId == positionId && cv.UserId == userId);
    }

    public async Task<CV> CreateCVAsync(int positionId, string userId, string title, Dictionary<int, string> attributeValues, int[] projectIds)
    {
        var cv = new CV
        {
            PositionId = positionId,
            UserId = userId,
            Title = title,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.CVs.Add(cv);
        await _context.SaveChangesAsync();

        // Add attribute values
        foreach (var kvp in attributeValues)
        {
            var cvAttribute = new CVAttribute
            {
                CVId = cv.Id,
                PositionAttributeId = kvp.Key,
                Value = kvp.Value
            };
            _context.CVAttributes.Add(cvAttribute);
        }

        // Add projects
        if (projectIds != null)
        {
            foreach (var projectId in projectIds)
            {
                var cvProject = new CVProject
                {
                    CVId = cv.Id,
                    ProjectId = projectId
                };
                _context.CVProjects.Add(cvProject);
            }
        }

        await _context.SaveChangesAsync();
        return cv;
    }

    public async Task<CV?> UpdateCVAsync(int id, string title, Dictionary<int, string> attributeValues, int[] projectIds)
    {
        var cv = await _context.CVs
            .Include(c => c.CVAttributes)
            .Include(c => c.CVProjects)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cv == null)
            return null;

        cv.Title = title;
        cv.UpdatedAt = DateTime.UtcNow;

        // Update attributes
        _context.CVAttributes.RemoveRange(cv.CVAttributes);

        foreach (var kvp in attributeValues)
        {
            var cvAttribute = new CVAttribute
            {
                CVId = cv.Id,
                PositionAttributeId = kvp.Key,
                Value = kvp.Value
            };
            _context.CVAttributes.Add(cvAttribute);
        }

        // Update projects
        _context.CVProjects.RemoveRange(cv.CVProjects);

        if (projectIds != null)
        {
            foreach (var projectId in projectIds)
            {
                var cvProject = new CVProject
                {
                    CVId = cv.Id,
                    ProjectId = projectId
                };
                _context.CVProjects.Add(cvProject);
            }
        }

        await _context.SaveChangesAsync();
        return cv;
    }

    public async Task<bool> DeleteCVAsync(int id)
    {
        var cv = await _context.CVs.FindAsync(id);
        if (cv == null)
            return false;

        _context.CVs.Remove(cv);
        await _context.SaveChangesAsync();
        return true;
    }
}
