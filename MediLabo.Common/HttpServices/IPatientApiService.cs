using MediLabo.Common.DTOs;

namespace MediLabo.Common.HttpServices;

public interface IPatientApiService
{
    Task<Result<PatientDto>> GetPatientByIdAsync(int patientId);
    Task<Result<PatientAssessmentDto>> GetPatientMedicalInfoAsync(int patientId);
    Task<Result<string>> GetPatientFullNameAsync(int patientId);
}