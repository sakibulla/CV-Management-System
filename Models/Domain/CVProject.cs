namespace CVManagementSystem.Models.Domain;

public class CVProject
{
    public int Id { get; set; }
    public int CVId { get; set; }
    public int ProjectId { get; set; }

    // Relationships
    public CV? CV { get; set; }
    public Project? Project { get; set; }
}
