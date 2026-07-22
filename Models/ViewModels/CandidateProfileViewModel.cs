namespace CVManagementSystem.Models.ViewModels;

public class CandidateProfileViewModel
{
    public int ProfileId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public Dictionary<int, string> AttributeValues { get; set; } = new();
    public List<AttributeDisplayViewModel> AvailableAttributes { get; set; } = new();
    public DateTime UpdatedAt { get; set; }
}
