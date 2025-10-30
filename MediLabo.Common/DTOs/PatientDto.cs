namespace MediLabo.Common.DTOs;

public class PatientDto
{
    public int Id { get; init; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    public string FullName => $"{FirstName} {LastName}";
}
