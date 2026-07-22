namespace CVManagementSystem.Models.Domain;

public class CV
{
    public int Id { get; set; }
    public int PositionId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Relationships
    public Position? Position { get; set; }
    public ApplicationUser? User { get; set; }
    public ICollection<CVAttribute> CVAttributes { get; set; } = new List<CVAttribute>();
    public ICollection<CVProject> CVProjects { get; set; } = new List<CVProject>();
}
