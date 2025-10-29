using MediLabo.Assessments.API.Calculators;
using MediLabo.Assessments.API.Models;
using MediLabo.Common;
using MediLabo.Common.DTOs;
using MediLabo.Common.HttpServices;

namespace MediLabo.Assessments.API.Services;

public class DiabetesRiskService
{
    private readonly IApiService _apiService;
    private readonly AgeCalculator _ageCalculator;
    private readonly TriggerTermsCalculator _triggerTermsCalculator;
    private readonly DiabetesRiskCalculator _diabetesRiskCalculator;
    private readonly ILogger<DiabetesRiskService> _logger;

    public DiabetesRiskService(IApiService apiService, AgeCalculator ageCalculator, TriggerTermsCalculator triggerTermsCalculator, DiabetesRiskCalculator diabetesRiskCalculator, ILogger<DiabetesRiskService> logger)
    {
        _apiService = apiService;
        _ageCalculator = ageCalculator;
        _triggerTermsCalculator = triggerTermsCalculator;
        _diabetesRiskCalculator = diabetesRiskCalculator;
        _logger = logger;
    }

    public async Task<Result<AssessmentResult>> CalculateRiskAsync(int patientId)
    {
        _logger.LogInformation("Calculating diabetes risk for patient {PatientId}", patientId);

        var patientResult = await _apiService.GetAsync<PatientAssessmentDto>($"api/patients/{patientId}");

        if (!patientResult.IsSuccess)
        {
            _logger.LogWarning("Failed to retrieve patient {PatientId}: {Error}", patientId, patientResult.Error);
            return Result<AssessmentResult>.Failure(patientResult.Error!);
        }

        var patient = patientResult.Value!;

        var notesResult = await _apiService.GetAsync<IEnumerable<NoteDto>>($"api/notes/patient/{patientId}");

        if (!notesResult.IsSuccess)
        {
            _logger.LogWarning("Failed to retrieve notes for patient {PatientId}: {Error}", patientId, notesResult.Error);
            return Result<AssessmentResult>.Failure(notesResult.Error!);
        }

        var notes = notesResult.Value!;

        var age = _ageCalculator.CalculateAge(patient.BirthDate);

        var noteContents = notes.Select(n => n.Content).ToList();
        var triggerCount = _triggerTermsCalculator.CountTriggers(noteContents);

        var riskInput = new RiskInput
        {
            Age = age,
            Gender = patient.GenderId == 1 ? Gender.Male : Gender.Female,
            TriggerCount = triggerCount
        };

        var riskLevel = _diabetesRiskCalculator.CalculateRisk(riskInput);

        var result = new AssessmentResult
        {
            PatientId = patientId,
            RiskLevel = riskLevel,
            TriggerCount = triggerCount,
            Age = age,
            Gender = riskInput.Gender
        };

        _logger.LogInformation(
            "Diabetes risk calculated for patient {PatientId}: {RiskLevel} (Age: {Age}, Triggers: {TriggerCount})",
            patientId, riskLevel, age, triggerCount);

        return Result<AssessmentResult>.Success(result);
    }
}