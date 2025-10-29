using MediLabo.Common;
using MediLabo.Web.Models.ViewModels;

namespace MediLabo.Web.Services;

public class NoteService
{
    private readonly IApiService _apiService;
    private readonly ILogger<NoteService> _logger;

    public NoteService(IApiService apiService, ILogger<NoteService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<NoteViewModel>>> GetPatientNotesAsync(int patientId)
    {
        _logger.LogInformation("Fetching notes for patient {PatientId}", patientId);

        var result = await _apiService.GetAsync<IEnumerable<NoteViewModel>>($"/api/notes/patient/{patientId}");

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to fetch notes for patient {PatientId}: {Error}", patientId, result.Error);
        }

        return result;
    }

    public async Task<Result<NoteViewModel>> GetNoteByIdAsync(string noteId)
    {
        _logger.LogInformation("Fetching note {NoteId}", noteId);

        var result = await _apiService.GetAsync<NoteViewModel>($"/api/notes/{noteId}");

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to fetch note {NoteId}: {Error}", noteId, result.Error);
        }

        return result;
    }

    public async Task<Result<NoteViewModel>> CreateNoteAsync(CreateNoteViewModel model)
    {
        _logger.LogInformation("Creating note for patient {PatientId}", model.PatientId);

        var result = await _apiService.PostAsync<CreateNoteViewModel, NoteViewModel>("/api/notes", model);

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to create note for patient {PatientId}: {Error}", model.PatientId, result.Error);
        }

        return result;
    }

    public async Task<Result<NoteViewModel>> UpdateNoteAsync(string noteId, CreateNoteViewModel model)
    {
        _logger.LogInformation("Updating note {NoteId}", noteId);

        var result = await _apiService.PutAsync<CreateNoteViewModel, NoteViewModel>($"/api/notes/{noteId}", model);

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to update note {NoteId}: {Error}", noteId, result.Error);
        }

        return result;
    }

    public async Task<Result<bool>> DeleteNoteAsync(string noteId)
    {
        _logger.LogInformation("Deleting note {NoteId}", noteId);

        var result = await _apiService.DeleteAsync($"/api/notes/{noteId}");

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to delete note {NoteId}: {Error}", noteId, result.Error);
        }

        return result;
    }
}
