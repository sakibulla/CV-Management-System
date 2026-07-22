namespace CVManagementSystem.Models.ViewModels;

public class CVListViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string PositionTitle { get; set; } = string.Empty;
    public int PositionId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CVFormViewModel
{
    public int Id { get; set; }
    public int PositionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string PositionTitle { get; set; } = string.Empty;
    public Dictionary<int, string> AttributeValues { get; set; } = new();
    public List<CVAttributeViewModel> Attributes { get; set; } = new();
    public List<ProjectSelectionViewModel> AvailableProjects { get; set; } = new();
    public int[] SelectedProjectIds { get; set; } = Array.Empty<int>();
}

public class CVAttributeViewModel
{
    public int PositionAttributeId { get; set; }
    public string AttributeName { get; set; } = string.Empty;
    public string? CurrentValue { get; set; }
}

public class ProjectSelectionViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
}

public class CVDetailViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int PositionId { get; set; }
    public string PositionTitle { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<CVAttributeDetailViewModel> Attributes { get; set; } = new();
    public List<ProjectDetailViewModel> Projects { get; set; } = new();
}

public class CVAttributeDetailViewModel
{
    public string AttributeName { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class ProjectDetailViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Technologies { get; set; } = string.Empty;
}
