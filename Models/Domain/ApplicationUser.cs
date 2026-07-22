using Microsoft.AspNetCore.Identity;

namespace CVManagementSystem.Models.Domain;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsBlocked { get; set; } = false;

    // Relationships
    public CandidateProfile? CandidateProfile { get; set; }
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<CV> CVs { get; set; } = new List<CV>();
    public ICollection<Position> CreatedPositions { get; set; } = new List<Position>();
    public ICollection<PositionAttribute> CreatedAttributes { get; set; } = new List<PositionAttribute>();
}
