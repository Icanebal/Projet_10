namespace MediLabo.Patients.API.Models.DTOs
{
    public class PatientDto
    {
        public int Id { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public DateTime BirthDate { get; init; }
        public int GenderId { get; init; }
        public required string GenderName { get; init; }
        public string? Address { get; init; }
        public string? Phone { get; init; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
