using MediLabo.Common;
using MediLabo.Common.Models;
using MediLabo.Common.HttpServices;
using MediLabo.Common.DTOs;

namespace MediLabo.Web.Services;

public class AssessmentService
{
    private readonly IApiService _apiService;
    private readonly ILogger<AssessmentService> _logger;
    public AssessmentService(IApiService apiService, ILogger<AssessmentService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<Result<DiabetesRiskLevel>> GetDiabetesRiskAsync(int patientId)
    {
        _logger.LogInformation("Retrieving diabetes risk for patient {PatientId}", patientId);

        var result = await _apiService.GetAsync<DiabetesRiskResponse>($"/api/assessments/diabetes/{patientId}");

        if (result.IsFailure)
        {
            _logger.LogError("Failed to retrieve diabetes risk for patient {PatientId}: {Error}",
                patientId, result.Error);
            return Result<DiabetesRiskLevel>.Failure(result.Error!);
        }

        _logger.LogInformation("Diabetes risk for patient {PatientId}: {RiskLevel}",
            patientId, result.Value!.RiskLevel);

        return Result<DiabetesRiskLevel>.Success(result.Value.RiskLevel);
    }
}