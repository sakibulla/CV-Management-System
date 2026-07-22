namespace CVManagementSystem.Models.ViewModels;

public class ProjectViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Technologies { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class ProjectFormViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Technologies { get; set; } = string.Empty;
}
