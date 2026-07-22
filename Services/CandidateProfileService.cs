using Microsoft.EntityFrameworkCore;
using CVManagementSystem.Data;
using CVManagementSystem.Models.Domain;

namespace CVManagementSystem.Services;

public class CandidateProfileService
{
    private readonly AppDbContext _context;

    public CandidateProfileService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CandidateProfile> GetOrCreateProfileAsync(string userId)
    {
        var profile = await _context.CandidateProfiles
            .Include(p => p.ProfileAttributes)
            .ThenInclude(pa => pa.PositionAttribute)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null)
        {
            profile = new CandidateProfile
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.CandidateProfiles.Add(profile);
            await _context.SaveChangesAsync();
        }

        return profile;
    }

    public async Task<CandidateProfile?> GetProfileWithAttributesAsync(string userId)
    {
        return await _context.CandidateProfiles
            .Include(p => p.User)
            .Include(p => p.ProfileAttributes)
            .ThenInclude(pa => pa.PositionAttribute)
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<IEnumerable<PositionAttribute>> GetAllAttributesAsync()
    {
        return await _context.PositionAttributes.ToListAsync();
    }

    public async Task<CandidateProfile?> UpdateProfileAsync(string userId, string firstName, string lastName, string location)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return null;

        user.FirstName = firstName;
        user.LastName = lastName;

        var profile = await GetOrCreateProfileAsync(userId);
        profile.Location = location;
        profile.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return profile;
    }

    public async Task UpdateProfileAttributesAsync(string userId, Dictionary<int, string> attributeValues)
    {
        var profile = await GetOrCreateProfileAsync(userId);

        // Remove existing profile attributes
        var existingAttributes = _context.ProfileAttributes
            .Where(pa => pa.CandidateProfileId == profile.Id);
        _context.ProfileAttributes.RemoveRange(existingAttributes);

        // Add new ones
        foreach (var kvp in attributeValues)
        {
            if (!string.IsNullOrEmpty(kvp.Value))
            {
                var profileAttribute = new ProfileAttribute
                {
                    CandidateProfileId = profile.Id,
                    PositionAttributeId = kvp.Key,
                    Value = kvp.Value
                };
                _context.ProfileAttributes.Add(profileAttribute);
            }
        }

        profile.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
}
