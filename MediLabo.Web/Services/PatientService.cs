using MediLabo.Common;
using MediLabo.Web.Models.ViewModels;

namespace MediLabo.Web.Services;

// Service spécifique pour gérer les appels API liés aux patients
public class PatientService
{
    private readonly ApiService _apiService;
    private readonly ILogger<PatientService> _logger;
    private const string PatientsEndpoint = "/api/patients";

    public PatientService(ApiService apiService, ILogger<PatientService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<Result<List<PatientViewModel>>> GetAllPatientsAsync()
    {
        _logger.LogInformation("Attempting to retrieve all patients");

        var result = await _apiService.GetAsync<List<PatientViewModel>>(PatientsEndpoint);

        if (result.IsFailure)
        {
            _logger.LogError("Failed to retrieve all patients: {Error}", result.Error);
            return Result<List<PatientViewModel>>.Failure(result.Error ?? "Unknown error");
        }

        _logger.LogInformation("Successfully retrieved {PatientCount} patients", result.Value?.Count ?? 0);
        return result;
    }

    public async Task<Result<PatientViewModel>> GetPatientByIdAsync(int patientId)
    {
        _logger.LogInformation("Attempting to retrieve patient with ID {PatientId}", patientId);

        var result = await _apiService.GetAsync<PatientViewModel>($"{PatientsEndpoint}/{patientId}");

        if (result.IsFailure)
        {
            _logger.LogError("Failed to retrieve patient with ID {PatientId}: {Error}", patientId, result.Error);
            return Result<PatientViewModel>.Failure(result.Error ?? "Unknown error");
        }

        _logger.LogInformation("Successfully retrieved patient with ID {PatientId}", patientId);
        return result;
    }

    public async Task<Result<PatientViewModel>> CreatePatientAsync(CreatePatientViewModel createPatientViewModel)
    {
        _logger.LogInformation("Attempting to create new patient: {FirstName} {LastName}",
            createPatientViewModel.FirstName,
            createPatientViewModel.LastName);

        var result = await _apiService.PostAsync<CreatePatientViewModel, PatientViewModel>(
            PatientsEndpoint,
            createPatientViewModel);

        if (result.IsFailure)
        {
            _logger.LogError("Failed to create patient: {Error}", result.Error);
            return Result<PatientViewModel>.Failure(result.Error ?? "Unknown error");
        }

        _logger.LogInformation("Successfully created patient with ID {PatientId}", result.Value?.Id);
        return result;
    }

    public async Task<Result<PatientViewModel>> UpdatePatientAsync(int patientId, CreatePatientViewModel updatePatientViewModel)
    {
        _logger.LogInformation("Attempting to update patient with ID {PatientId}", patientId);

        var result = await _apiService.PutAsync<CreatePatientViewModel, PatientViewModel>(
            $"{PatientsEndpoint}/{patientId}",
            updatePatientViewModel);

        if (result.IsFailure)
        {
            _logger.LogError("Failed to update patient with ID {PatientId}: {Error}", patientId, result.Error);
            return Result<PatientViewModel>.Failure(result.Error ?? "Unknown error");
        }

        _logger.LogInformation("Successfully updated patient with ID {PatientId}", patientId);
        return result;
    }

    public async Task<Result<bool>> DeletePatientAsync(int patientId)
    {
        _logger.LogInformation("Attempting to delete patient with ID {PatientId}", patientId);

        var result = await _apiService.DeleteAsync($"{PatientsEndpoint}/{patientId}");

        if (result.IsFailure)
        {
            _logger.LogError("Failed to delete patient with ID {PatientId}: {Error}", patientId, result.Error);
            return Result<bool>.Failure(result.Error ?? "Unknown error");
        }

        _logger.LogInformation("Successfully deleted patient with ID {PatientId}", patientId);
        return result;
    }
}