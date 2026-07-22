namespace CVManagementSystem.Models.ViewModels;

public class SearchResultsViewModel
{
    public string Query { get; set; } = string.Empty;
    public List<SearchPositionResultViewModel> Positions { get; set; } = new();
    public List<SearchCVResultViewModel> CVs { get; set; } = new();
}

public class SearchPositionResultViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class SearchCVResultViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string PositionTitle { get; set; } = string.Empty;
    public string CandidateName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
