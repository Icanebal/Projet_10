using MediLabo.Patients.API.Models.Entities;

namespace MediLabo.Patients.API.Interfaces
{
    public interface IPatientRepository
    {
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<Patient?> GetByIdAsync(int id);
        Task<Patient> CreateAsync(Patient patient);
        Task<Patient> UpdateAsync(Patient patient);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Gender>> GetAllGendersAsync();
        Task<bool> GenderExistsAsync(int genderId);
    }
}
