namespace CVManagementSystem.Models.Domain;

public class ProfileAttribute
{
    public int Id { get; set; }
    public int CandidateProfileId { get; set; }
    public int PositionAttributeId { get; set; }
    public string Value { get; set; } = string.Empty;

    // Relationships
    public CandidateProfile? CandidateProfile { get; set; }
    public PositionAttribute? PositionAttribute { get; set; }
}
