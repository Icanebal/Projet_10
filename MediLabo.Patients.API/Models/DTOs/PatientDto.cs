namespace MediLabo.Patients.API.Models.DTOs
{
    public class PatientDto
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public required string Gender { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
    }
}
