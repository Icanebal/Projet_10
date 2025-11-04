using MediLabo.Common;
using MediLabo.Common.DTOs;
using MediLabo.Common.HttpServices;

namespace MediLabo.Notes.API.Services;

public class PatientService : IPatientService
{
    private readonly IApiService _apiService;
    private readonly ILogger<PatientService> _logger;

    public PatientService(IApiService apiService, ILogger<PatientService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<Result<PatientDto>> GetPatientAsync(int patientId)
    {
        _logger.LogInformation("Retrieving patient {PatientId} from Patients API", patientId);

        var result = await _apiService.GetAsync<PatientDto>($"api/patients/{patientId}");

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to retrieve patient {PatientId}: {Error}", patientId, result.Error);
        }

        return result;
    }
}
