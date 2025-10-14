namespace MediLabo.Patients.API.Models.Entities
{
    public class Gender
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public ICollection<Patient> Patients { get; set; } = new List<Patient>();
    }
}
