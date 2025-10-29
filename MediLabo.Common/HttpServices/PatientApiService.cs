using MediLabo.Common.DTOs;
using Microsoft.Extensions.Logging;

namespace MediLabo.Common.HttpServices;

public class PatientApiService : BaseApiService, IPatientApiService
{
    public PatientApiService(HttpClient httpClient, ILogger<PatientApiService> logger) 
        : base(httpClient, logger)
    {
    }

    public async Task<Result<PatientDto>> GetPatientByIdAsync(int patientId)
    {
        return await GetAsync<PatientDto>($"api/patients/{patientId}", $"Fetching patient {patientId}");
    }

    public async Task<Result<PatientAssessmentDto>> GetPatientMedicalInfoAsync(int patientId)
    {
        return await GetAsync<PatientAssessmentDto>($"api/patients/{patientId}", $"Fetching medical info for patient {patientId}");
    }

    public async Task<Result<string>> GetPatientFullNameAsync(int patientId)
    {
        var result = await GetPatientByIdAsync(patientId);
        
        if (!result.IsSuccess)
        {
            return Result<string>.Failure(result.Error!);
        }

        return Result<string>.Success(result.Value!.FullName);
    }
}
