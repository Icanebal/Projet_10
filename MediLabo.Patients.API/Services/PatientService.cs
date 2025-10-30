using MediLabo.Patients.API.Utilities;
using MediLabo.Patients.API.Models.DTOs;
using MediLabo.Patients.API.Interfaces;
using MediLabo.Common;

namespace MediLabo.Patients.API.Services
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
            _logger.LogInformation("Retrieving all patients");

            var result = await _patientRepository.GetAllAsync();

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to retrieve patients. Error: {Error}", result.Error);
                return Result<IEnumerable<PatientDto>>.Failure(result.Error!);
            }

            var patientDtos = Mapping.MapToDtoCollection(result.Value!);
            _logger.LogInformation("Successfully retrieved {PatientCount} patients", patientDtos.Count());

            return Result<IEnumerable<PatientDto>>.Success(patientDtos);
        }

        public async Task<Result<PatientDto>> GetByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving patient ID: {PatientId}", id);

            var result = await _patientRepository.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to retrieve patient ID: {PatientId}. Error: {Error}", id, result.Error);
                return Result<PatientDto>.Failure(result.Error!);
            }

            _logger.LogInformation("Patient ID: {PatientId} retrieved successfully", id);
            return Result<PatientDto>.Success(Mapping.MapToDto(result.Value!));
        }

        public async Task<Result<PatientDto>> CreateAsync(CreatePatientDto createDto)
        {
            _logger.LogInformation("Creating patient: {FirstName} {LastName}", createDto.FirstName, createDto.LastName);

            var genderResult = await _patientRepository.GenderExistsAsync(createDto.GenderId);
            if (!genderResult.IsSuccess)
            {
                _logger.LogWarning("Failed to create patient - Gender ID: {GenderId} not found", createDto.GenderId);
                return Result<PatientDto>.Failure(genderResult.Error!);
            }

            var patient = Mapping.MapToEntity(createDto);
            var createResult = await _patientRepository.CreateAsync(patient);

            if (!createResult.IsSuccess)
            {
                _logger.LogError("Failed to create patient. Error: {Error}", createResult.Error);
                return Result<PatientDto>.Failure(createResult.Error!);
            }

            _logger.LogInformation("Patient created successfully - ID: {PatientId}", createResult.Value!.Id);
            return Result<PatientDto>.Success(Mapping.MapToDto(createResult.Value!));
        }

        public async Task<Result<PatientDto>> UpdateAsync(int id, CreatePatientDto updateDto)
        {
            _logger.LogInformation("Updating patient ID: {PatientId}", id);

            var existingResult = await _patientRepository.GetByIdAsync(id);
            if (!existingResult.IsSuccess)
            {
                _logger.LogWarning("Failed to update patient ID: {PatientId}. Error: {Error}", id, existingResult.Error);
                return Result<PatientDto>.Failure(existingResult.Error!);
            }

            var genderResult = await _patientRepository.GenderExistsAsync(updateDto.GenderId);
            if (!genderResult.IsSuccess)
            {
                _logger.LogWarning("Failed to update patient - Gender ID: {GenderId} not found", updateDto.GenderId);
                return Result<PatientDto>.Failure(genderResult.Error!);
            }

            Mapping.MapUpdateToEntity(updateDto, existingResult.Value!);

            var updateResult = await _patientRepository.UpdateAsync(existingResult.Value!);

            if (!updateResult.IsSuccess)
            {
                _logger.LogError("Failed to update patient ID: {PatientId}. Error: {Error}", id, updateResult.Error);
                return Result<PatientDto>.Failure(updateResult.Error!);
            }

            _logger.LogInformation("Patient ID: {PatientId} updated successfully", id);
            return Result<PatientDto>.Success(Mapping.MapToDto(updateResult.Value!));
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting patient ID: {PatientId}", id);

            var result = await _patientRepository.DeleteAsync(id);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to delete patient ID: {PatientId}. Error: {Error}", id, result.Error);
                return Result<bool>.Failure(result.Error!);
            }

            _logger.LogInformation("Patient ID: {PatientId} deleted successfully", id);
            return Result<bool>.Success(true);
        }

        public async Task<Result<IEnumerable<GenderDto>>> GetAllGendersAsync()
        {
            _logger.LogInformation("Retrieving all genders");

            var result = await _patientRepository.GetAllGendersAsync();

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Failed to retrieve genders. Error: {Error}", result.Error);
                return Result<IEnumerable<GenderDto>>.Failure(result.Error!);
            }

            var genderDtos = Mapping.MapToGenderDtoCollection(result.Value!);
            _logger.LogInformation("Successfully retrieved {GenderCount} genders", genderDtos.Count());

            return Result<IEnumerable<GenderDto>>.Success(genderDtos);
        }
    }
}
