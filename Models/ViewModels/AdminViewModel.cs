namespace CVManagementSystem.Models.ViewModels;

public class UserListViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsBlocked { get; set; }
    public DateTime? LastLogin { get; set; }
}

public class UserDetailViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string CurrentRole { get; set; } = string.Empty;
    public bool IsBlocked { get; set; }
    public List<string> AvailableRoles { get; set; } = new();
}

public class AssignRoleViewModel
{
    public string UserId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
}

public class AdminEditProfileViewModel
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public List<AttributeDisplayViewModel> ProfileAttributes { get; set; } = new();
    public Dictionary<int, string>? AttributeValues { get; set; }
}

public class AdminEditCVViewModel
{
    public int Id { get; set; }
    public int PositionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string PositionTitle { get; set; } = string.Empty;
    public string CandidateName { get; set; } = string.Empty;
    public List<CVAttributeViewModel> Attributes { get; set; } = new();
    public List<ProjectSelectionViewModel> AvailableProjects { get; set; } = new();
    public int[] SelectedProjectIds { get; set; } = Array.Empty<int>();
    public Dictionary<int, string>? AttributeValues { get; set; }
}

public class AttributeDisplayViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? CurrentValue { get; set; }
}
