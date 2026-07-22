using Microsoft.EntityFrameworkCore;
using CVManagementSystem.Data;
using CVManagementSystem.Models.Domain;

namespace CVManagementSystem.Services;

public class ProjectService
{
    private readonly AppDbContext _context;

    public ProjectService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Project>> GetProjectsByUserIdAsync(string userId)
    {
        return await _context.Projects
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<Project?> GetProjectByIdAsync(int id)
    {
        return await _context.Projects
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Project> CreateProjectAsync(string userId, string name, string description, string technologies)
    {
        var project = new Project
        {
            UserId = userId,
            Name = name,
            Description = description,
            Technologies = technologies,
            CreatedAt = DateTime.UtcNow
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task<Project?> UpdateProjectAsync(int id, string name, string description, string technologies)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
            return null;

        project.Name = name;
        project.Description = description;
        project.Technologies = technologies;

        await _context.SaveChangesAsync();
        return project;
    }

    public async Task<bool> DeleteProjectAsync(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
            return false;

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return true;
    }
}
