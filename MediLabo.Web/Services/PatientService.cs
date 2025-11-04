using MediLabo.Common;
using MediLabo.Web.Models.ViewModels;
using MediLabo.Common.HttpServices;

namespace MediLabo.Web.Services;

public class PatientService
{
    private readonly IApiService _apiService;
    private readonly ILogger<PatientService> _logger;
    private const string PatientsEndpoint = "/api/patients";

    public PatientService(IApiService apiService, ILogger<PatientService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<Result<List<PatientViewModel>>> GetAllPatientsAsync()
    {
        _logger.LogInformation("Retrieving all patients");
        var result = await _apiService.GetAsync<List<PatientViewModel>>(PatientsEndpoint);
        
        _logger.Log(result.IsSuccess ? LogLevel.Information : LogLevel.Error,
            "Get all patients: {IsSuccess}, Count: {Count}, Error: {Error}",
            result.IsSuccess, result.Value?.Count, result.Error);

        return result;
    }

    public async Task<Result<PatientViewModel>> GetPatientByIdAsync(int patientId)
    {
        _logger.LogInformation("Retrieving patient with ID {PatientId}", patientId);
        var result = await _apiService.GetAsync<PatientViewModel>($"{PatientsEndpoint}/{patientId}");
        
        _logger.Log(result.IsSuccess ? LogLevel.Information : LogLevel.Error,
            "Get patient {PatientId}: {IsSuccess}, Error: {Error}",
            patientId, result.IsSuccess, result.Error);

        return result;
    }

    public async Task<Result<PatientViewModel>> CreatePatientAsync(CreatePatientViewModel createPatientViewModel)
    {
        _logger.LogInformation("Creating new patient: {FirstName} {LastName}",
            createPatientViewModel.FirstName, createPatientViewModel.LastName);

        var result = await _apiService.PostAsync<CreatePatientViewModel, PatientViewModel>(
            PatientsEndpoint, createPatientViewModel);

        _logger.Log(result.IsSuccess ? LogLevel.Information : LogLevel.Error,
            "Create patient: {IsSuccess}, PatientId: {PatientId}, Error: {Error}",
            result.IsSuccess, result.Value?.Id, result.Error);

        return result;
    }

    public async Task<Result<PatientViewModel>> UpdatePatientAsync(int patientId, UpdatePatientViewModel updatePatientViewModel)
    {
        _logger.LogInformation("Updating patient with ID {PatientId}", patientId);

        var result = await _apiService.PutAsync<UpdatePatientViewModel, PatientViewModel>(
            $"{PatientsEndpoint}/{patientId}", updatePatientViewModel);

        _logger.Log(result.IsSuccess ? LogLevel.Information : LogLevel.Error,
            "Update patient {PatientId}: {IsSuccess}, Error: {Error}",
            patientId, result.IsSuccess, result.Error);

        return result;
    }

    public async Task<Result<bool>> DeletePatientAsync(int patientId)
    {
        _logger.LogInformation("Deleting patient with ID {PatientId}", patientId);
        var result = await _apiService.DeleteAsync($"{PatientsEndpoint}/{patientId}");

        _logger.Log(result.IsSuccess ? LogLevel.Information : LogLevel.Error,
            "Delete patient {PatientId}: {IsSuccess}, Error: {Error}",
            patientId, result.IsSuccess, result.Error);

        return result;
    }

    public async Task<Result<List<GenderViewModel>>> GetAllGendersAsync()
    {
        _logger.LogInformation("Retrieving all genders");
        var result = await _apiService.GetAsync<List<GenderViewModel>>($"{PatientsEndpoint}/genders");

        _logger.Log(result.IsSuccess ? LogLevel.Information : LogLevel.Error,
            "Get all genders: {IsSuccess}, Count: {Count}, Error: {Error}",
            result.IsSuccess, result.Value?.Count, result.Error);

        return result;
    }
}
