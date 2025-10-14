namespace MediLabo.Patients.API.Models.DTOs
{
    public class PatientDto
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public int GenderId { get; set; }
        public required string GenderName { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
