using Microsoft.EntityFrameworkCore;
using CVManagementSystem.Data;
using CVManagementSystem.Models.Domain;

namespace CVManagementSystem.Services;

public class PositionService
{
    private readonly AppDbContext _context;

    public PositionService(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<Position> GetAllPositions()
    {
        return _context.Positions
            .Include(p => p.CreatedByUser)
            .Include(p => p.Attributes)
            .AsNoTracking();
    }

    public async Task<Position?> GetPositionByIdAsync(int id)
    {
        return await _context.Positions
            .Include(p => p.CreatedByUser)
            .Include(p => p.Attributes)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Position> CreatePositionAsync(string title, string description, string createdBy, int[] attributeIds)
    {
        var position = new Position
        {
            Title = title,
            Description = description,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Positions.Add(position);
        await _context.SaveChangesAsync();

        if (attributeIds != null && attributeIds.Length > 0)
        {
            var attributes = await _context.PositionAttributes
                .Where(a => attributeIds.Contains(a.Id))
                .ToListAsync();

            foreach (var attribute in attributes)
            {
                position.Attributes.Add(attribute);
            }

            await _context.SaveChangesAsync();
        }

        return position;
    }

    public async Task<Position?> UpdatePositionAsync(int id, string title, string description, int[] attributeIds)
    {
        var position = await _context.Positions
            .Include(p => p.Attributes)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (position == null)
            return null;

        position.Title = title;
        position.Description = description;
        position.UpdatedAt = DateTime.UtcNow;

        // Update attributes
        position.Attributes.Clear();
        if (attributeIds != null && attributeIds.Length > 0)
        {
            var attributes = await _context.PositionAttributes
                .Where(a => attributeIds.Contains(a.Id))
                .ToListAsync();

            foreach (var attribute in attributes)
            {
                position.Attributes.Add(attribute);
            }
        }

        await _context.SaveChangesAsync();
        return position;
    }

    public async Task<bool> DeletePositionAsync(int id)
    {
        var position = await _context.Positions.FindAsync(id);
        if (position == null)
            return false;

        _context.Positions.Remove(position);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Position?> DuplicatePositionAsync(int id, string userId)
    {
        var original = await _context.Positions
            .Include(p => p.Attributes)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (original == null)
            return null;

        var duplicate = new Position
        {
            Title = original.Title + " (Copy)",
            Description = original.Description,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Positions.Add(duplicate);
        await _context.SaveChangesAsync();

        // Duplicate attributes
        foreach (var attribute in original.Attributes)
        {
            duplicate.Attributes.Add(attribute);
        }

        await _context.SaveChangesAsync();
        return duplicate;
    }

    public async Task<IEnumerable<PositionAttribute>> GetPositionAttributesAsync(int id)
    {
        var position = await _context.Positions
            .Include(p => p.Attributes)
            .FirstOrDefaultAsync(p => p.Id == id);

        return position?.Attributes ?? new List<PositionAttribute>();
    }
}
