using Microsoft.EntityFrameworkCore;
using CVManagementSystem.Data;
using CVManagementSystem.Models.Domain;

namespace CVManagementSystem.Services;

public class AttributeService
{
    private readonly AppDbContext _context;

    public AttributeService(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<PositionAttribute> GetAllAttributes()
    {
        return _context.PositionAttributes
            .Include(a => a.CreatedByUser)
            .AsNoTracking();
    }

    public async Task<PositionAttribute?> GetAttributeByIdAsync(int id)
    {
        return await _context.PositionAttributes
            .Include(a => a.CreatedByUser)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<PositionAttribute> CreateAttributeAsync(string name, string type, string dropdownOptions, string createdBy)
    {
        var attribute = new PositionAttribute
        {
            Name = name,
            Type = type,
            DropdownOptions = dropdownOptions,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        _context.PositionAttributes.Add(attribute);
        await _context.SaveChangesAsync();
        return attribute;
    }

    public async Task<PositionAttribute?> UpdateAttributeAsync(int id, string name, string type, string dropdownOptions)
    {
        var attribute = await _context.PositionAttributes.FindAsync(id);
        if (attribute == null)
            return null;

        attribute.Name = name;
        attribute.Type = type;
        attribute.DropdownOptions = dropdownOptions;

        await _context.SaveChangesAsync();
        return attribute;
    }

    public async Task<bool> DeleteAttributeAsync(int id)
    {
        var attribute = await _context.PositionAttributes
            .Include(a => a.Positions)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (attribute == null)
            return false;

        // Check if attribute is in use
        if (attribute.Positions.Any())
            return false;

        _context.PositionAttributes.Remove(attribute);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsAttributeInUseAsync(int id)
    {
        var attribute = await _context.PositionAttributes
            .Include(a => a.Positions)
            .FirstOrDefaultAsync(a => a.Id == id);

        return attribute?.Positions.Any() ?? false;
    }
}
