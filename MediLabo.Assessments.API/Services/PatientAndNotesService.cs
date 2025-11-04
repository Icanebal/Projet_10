using MediLabo.Common;
using MediLabo.Common.DTOs;
using MediLabo.Assessments.API.Models;
using MediLabo.Common.HttpServices;

namespace MediLabo.Assessments.API.Services;

public class PatientAndNotesService : IPatientAndNotesService
{
    private readonly IApiService _apiService;
    private readonly ILogger<PatientAndNotesService> _logger;

    public PatientAndNotesService(IApiService apiService, ILogger<PatientAndNotesService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<Result<PatientAssessmentDto>> GetPatientAsync(int patientId)
    {
        _logger.LogInformation("Retrieving patient {PatientId} from Patients API", patientId);

        var result = await _apiService.GetAsync<PatientAssessmentDto>($"api/patients/{patientId}");

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to retrieve patient {PatientId}: {Error}", patientId, result.Error);
        }

        return result;
    }

    public async Task<Result<IEnumerable<NoteDto>>> GetPatientNotesAsync(int patientId)
    {
        _logger.LogInformation("Retrieving notes for patient {PatientId} from Notes API", patientId);

        var result = await _apiService.GetAsync<IEnumerable<NoteDto>>($"api/notes/patient/{patientId}");

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to retrieve notes for patient {PatientId}: {Error}", patientId, result.Error);
        }

        return result;
    }
}
