namespace CVManagementSystem.Models.Domain;

public class Attribute
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "Text"; // "Text", "Number", "Dropdown"
    public string DropdownOptions { get; set; } = string.Empty; // JSON if Type == "Dropdown"
    public string CreatedBy { get; set; } = string.Empty; // UserId
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relationships
    public ApplicationUser? CreatedByUser { get; set; }
    public ICollection<Position> Positions { get; set; } = new List<Position>();
    public ICollection<CVAttribute> CVAttributes { get; set; } = new List<CVAttribute>();
}
