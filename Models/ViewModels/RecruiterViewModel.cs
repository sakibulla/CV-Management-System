namespace CVManagementSystem.Models.ViewModels;

public class RecruiterCVListViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CandidateName { get; set; } = string.Empty;
    public string CandidateEmail { get; set; } = string.Empty;
    public string PositionTitle { get; set; } = string.Empty;
    public int PositionId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class RecruiterCVDetailViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CandidateName { get; set; } = string.Empty;
    public string CandidateEmail { get; set; } = string.Empty;
    public string CandidateLocation { get; set; } = string.Empty;
    public int PositionId { get; set; }
    public string PositionTitle { get; set; } = string.Empty;
    public string PositionDescription { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<CVAttributeDetailViewModel> Attributes { get; set; } = new();
    public List<ProjectDetailViewModel> Projects { get; set; } = new();
}
