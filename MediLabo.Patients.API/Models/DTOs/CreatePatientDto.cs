using System.ComponentModel.DataAnnotations;

namespace MediLabo.Patients.API.Models.DTOs
{
    public class CreatePatientDto
    {
        [Required(ErrorMessage = "First name is required")]
        public required string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required")]
        public required string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        [Required(ErrorMessage = "Gender is required")]
        public required string Gender { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
    }
}