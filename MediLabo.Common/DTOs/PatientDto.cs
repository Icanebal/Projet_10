namespace MediLabo.Common.DTOs;

public class PatientDto
{
    public int Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    
    public string FullName => $"{FirstName} {LastName}";
}
