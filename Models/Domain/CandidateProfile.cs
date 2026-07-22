namespace CVManagementSystem.Models.Domain;

public class CandidateProfile
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Relationships
    public ApplicationUser? User { get; set; }
    public ICollection<ProfileAttribute> ProfileAttributes { get; set; } = new List<ProfileAttribute>();
}
