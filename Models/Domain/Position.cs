namespace CVManagementSystem.Models.Domain;

public class Position
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty; // UserId
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Relationships
    public ApplicationUser? CreatedByUser { get; set; }
    public ICollection<PositionAttribute> Attributes { get; set; } = new List<PositionAttribute>();
    public ICollection<CV> CVs { get; set; } = new List<CV>();
}
