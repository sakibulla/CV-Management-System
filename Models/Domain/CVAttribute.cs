namespace CVManagementSystem.Models.Domain;

public class CVAttribute
{
    public int Id { get; set; }
    public int CVId { get; set; }
    public int PositionAttributeId { get; set; }
    public string Value { get; set; } = string.Empty;

    // Relationships
    public CV? CV { get; set; }
    public PositionAttribute? PositionAttribute { get; set; }
}
