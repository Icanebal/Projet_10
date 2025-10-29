using Microsoft.EntityFrameworkCore;
using MediLabo.Patients.API.Data;
using MediLabo.Patients.API.Interfaces;
using MediLabo.Patients.API.Models.Entities;
using MediLabo.Common;

namespace MediLabo.Patients.API.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly PatientDbContext _context;

        public PatientRepository(PatientDbContext context)
        {
            _context = context;
        }

        public async Task<Result<IEnumerable<Patient>>> GetAllAsync()
        {
            var patients = await _context.Patients
                .Include(p => p.Gender)
                .ToListAsync();

            return Result<IEnumerable<Patient>>.Success(patients);
        }

        public async Task<Result<Patient>> GetByIdAsync(int id)
        {
            var patient = await _context.Patients
                .Include(p => p.Gender)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null)
            {
                return Result<Patient>.Failure("Patient not found");
            }

            return Result<Patient>.Success(patient);
        }

        public async Task<Result<Patient>> CreateAsync(Patient patient)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            
            var createdPatient = await _context.Patients
                .Include(p => p.Gender)
                .FirstOrDefaultAsync(p => p.Id == patient.Id);

            return Result<Patient>.Success(createdPatient!);
        }

        public async Task<Result<Patient>> UpdateAsync(Patient patient)
        {
            _context.Entry(patient).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
            var updatedPatient = await _context.Patients
                .Include(p => p.Gender)
                .FirstOrDefaultAsync(p => p.Id == patient.Id);

            return Result<Patient>.Success(updatedPatient!);
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            
            if (patient == null)
            {
                return Result<bool>.Failure("Patient not found");
            }

            patient.IsDeleted = true;
            patient.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Result<bool>.Success(true);
        }

        public async Task<Result<IEnumerable<Gender>>> GetAllGendersAsync()
        {
            var genders = await _context.Genders.ToListAsync();
            return Result<IEnumerable<Gender>>.Success(genders);
        }

        public async Task<Result<bool>> GenderExistsAsync(int genderId)
        {
            var exists = await _context.Genders.AnyAsync(g => g.Id == genderId);
            
            if (!exists)
            {
                return Result<bool>.Failure("Gender not found");
            }

            return Result<bool>.Success(true);
        }
    }
}
