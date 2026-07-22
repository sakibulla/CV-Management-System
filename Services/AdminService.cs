using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CVManagementSystem.Data;
using CVManagementSystem.Models.Domain;

namespace CVManagementSystem.Services;

public class AdminService
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminService(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public IQueryable<ApplicationUser> GetAllUsers()
    {
        return _context.Users.AsNoTracking();
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    public async Task<bool> AssignRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return false;

        // Remove all roles
        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Any())
        {
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
        }

        // Add new role
        var result = await _userManager.AddToRoleAsync(user, roleName);
        return result.Succeeded;
    }

    public async Task<bool> RemoveRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return false;

        var result = await _userManager.RemoveFromRoleAsync(user, roleName);
        return result.Succeeded;
    }

    public async Task<bool> BlockUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return false;

        user.IsBlocked = true;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> UnblockUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return false;

        user.IsBlocked = false;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<string?> GetUserRoleAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return null;

        var roles = await _userManager.GetRolesAsync(user);
        return roles.FirstOrDefault();
    }

    public async Task<CandidateProfile> GetOrCreateCandidateProfileAsync(string userId)
    {
        var profile = await _context.CandidateProfiles
            .Include(cp => cp.ProfileAttributes)
            .ThenInclude(pa => pa.PositionAttribute)
            .FirstOrDefaultAsync(cp => cp.UserId == userId);

        if (profile == null)
        {
            profile = new CandidateProfile
            {
                UserId = userId,
                Location = string.Empty,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.CandidateProfiles.Add(profile);
            await _context.SaveChangesAsync();
        }

        return profile;
    }

    public async Task<IEnumerable<PositionAttribute>> GetAllAttributesAsync()
    {
        return await _context.PositionAttributes.ToListAsync();
    }

    public async Task UpdateCandidateProfileAsync(string userId, string location)
    {
        var profile = await GetOrCreateCandidateProfileAsync(userId);
        profile.Location = location;
        profile.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task UpdateProfileAttributesAsync(string userId, Dictionary<int, string> attributeValues)
    {
        var profile = await GetOrCreateCandidateProfileAsync(userId);

        // Remove existing attributes
        var existingAttributes = await _context.ProfileAttributes
            .Where(pa => pa.CandidateProfileId == profile.Id)
            .ToListAsync();
        _context.ProfileAttributes.RemoveRange(existingAttributes);

        // Add new attributes
        foreach (var kvp in attributeValues)
        {
            if (!string.IsNullOrWhiteSpace(kvp.Value))
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

        await _context.SaveChangesAsync();
    }

    public async Task<CV?> GetCVByIdAsync(int cvId)
    {
        return await _context.CVs
            .Include(cv => cv.Position)
            .ThenInclude(p => p!.Attributes)
            .Include(cv => cv.User)
            .Include(cv => cv.CVAttributes)
            .ThenInclude(ca => ca.PositionAttribute)
            .Include(cv => cv.CVProjects)
            .ThenInclude(cp => cp.Project)
            .FirstOrDefaultAsync(cv => cv.Id == cvId);
    }

    public async Task<IEnumerable<Project>> GetProjectsByUserIdAsync(string userId)
    {
        return await _context.Projects
            .Where(p => p.UserId == userId)
            .ToListAsync();
    }

    public async Task UpdateCVAsync(int cvId, string title, Dictionary<int, string> attributeValues, int[] projectIds)
    {
        var cv = await _context.CVs
            .Include(c => c.CVAttributes)
            .Include(c => c.CVProjects)
            .FirstOrDefaultAsync(c => c.Id == cvId);

        if (cv == null)
            return;

        cv.Title = title;
        cv.UpdatedAt = DateTime.UtcNow;

        // Update attributes
        _context.CVAttributes.RemoveRange(cv.CVAttributes);

        foreach (var kvp in attributeValues)
        {
            if (!string.IsNullOrWhiteSpace(kvp.Value))
            {
                var cvAttribute = new CVAttribute
                {
                    CVId = cv.Id,
                    PositionAttributeId = kvp.Key,
                    Value = kvp.Value
                };
                _context.CVAttributes.Add(cvAttribute);
            }
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
    }
}
