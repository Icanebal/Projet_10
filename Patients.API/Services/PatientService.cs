using Patients.API.Utilities;
using Patients.API.Models.DTOs;
using Patients.API.Interfaces;

namespace Patients.API.Service
{
    public class PatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly ILogger<PatientService> _logger;

        public PatientService(IPatientRepository patientRepository, ILogger<PatientService> logger)
        {
            _patientRepository = patientRepository;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<PatientDto>>> GetAllAsync()
        {
            _logger.LogInformation("Attempting to retrieve all patients");

            var patients = await _patientRepository.GetAllAsync();
            var patientDtos = Mapping.MapToDtoCollection(patients);

            _logger.LogInformation("Successfully retrieved {PatientCount} patients", patientDtos.Count());
            return Result<IEnumerable<PatientDto>>.Success(patientDtos);
        }

        public async Task<Result<PatientDto>> GetByIdAsync(int id)
        {
            _logger.LogInformation("Attempting to retrieve patient with ID {PatientId}", id);

            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
            {
                _logger.LogWarning("Patient with ID {PatientId} not found", id);
                return Result<PatientDto>.Failure($"Patient with ID {id} not found");
            }

            _logger.LogInformation("Successfully retrieved patient with ID {PatientId}", id);
            return Result<PatientDto>.Success(Mapping.MapToDto(patient));
        }

        public async Task<Result<PatientDto>> CreateAsync(CreatePatientDto createDto)
        {
            _logger.LogInformation("Attempting to create new patient: {FirstName} {LastName}",
                createDto.FirstName, createDto.LastName);

            // Validation métier (exemple)
            if (createDto.BirthDate > DateTime.Now.AddYears(-1))
            {
                _logger.LogWarning("Invalid birth date: patient must be at least 1 year old");
                return Result<PatientDto>.Failure("Patient must be at least 1 year old");
            }

            var patient = Mapping.MapToEntity(createDto);
            var createdPatient = await _patientRepository.CreateAsync(patient);

            _logger.LogInformation("Successfully created patient with ID {PatientId}", createdPatient.Id);
            return Result<PatientDto>.Success(Mapping.MapToDto(createdPatient));
        }

        public async Task<Result<PatientDto>> UpdateAsync(int id, CreatePatientDto updateDto)
        {
            _logger.LogInformation("Attempting to update patient with ID {PatientId}", id);

            var existingPatient = await _patientRepository.GetByIdAsync(id);
            if (existingPatient == null)
            {
                _logger.LogWarning("Cannot update - patient with ID {PatientId} not found", id);
                return Result<PatientDto>.Failure($"Patient with ID {id} not found");
            }

            Mapping.MapUpdateToEntity(updateDto, existingPatient);

            var updatedPatient = await _patientRepository.UpdateAsync(existingPatient);

            _logger.LogInformation("Successfully updated patient with ID {PatientId}", id);
            return Result<PatientDto>.Success(Mapping.MapToDto(updatedPatient));
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            _logger.LogInformation("Attempting to delete patient with ID {PatientId}", id);

            var success = await _patientRepository.DeleteAsync(id);
            if (!success)
            {
                _logger.LogWarning("Cannot delete - patient with ID {PatientId} not found", id);
                return Result<bool>.Success(false);
            }

            _logger.LogInformation("Successfully deleted patient with ID {PatientId}", id);
            return Result<bool>.Success(true);
        }
    }
}
