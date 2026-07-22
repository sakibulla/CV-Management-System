namespace CVManagementSystem.Models.Domain;

public class Project
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Technologies { get; set; } = string.Empty; // comma-separated
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relationships
    public ApplicationUser? User { get; set; }
    public ICollection<CVProject> CVProjects { get; set; } = new List<CVProject>();
}
