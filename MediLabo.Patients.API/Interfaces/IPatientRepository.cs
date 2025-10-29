using MediLabo.Patients.API.Models.Entities;
using MediLabo.Common;

namespace MediLabo.Patients.API.Interfaces
{
    public interface IPatientRepository
    {
        Task<Result<IEnumerable<Patient>>> GetAllAsync();
        Task<Result<Patient>> GetByIdAsync(int id);
        Task<Result<Patient>> CreateAsync(Patient patient);
        Task<Result<Patient>> UpdateAsync(Patient patient);
        Task<Result<bool>> DeleteAsync(int id);
        Task<Result<IEnumerable<Gender>>> GetAllGendersAsync();
        Task<Result<bool>> GenderExistsAsync(int genderId);
    }
}
