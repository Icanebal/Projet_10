using MediLabo.Common.DTOs;
using Microsoft.Extensions.Logging;

namespace MediLabo.Common.HttpServices;

public class NoteApiService : BaseApiService, INoteApiService
{
    public NoteApiService(HttpClient httpClient, ILogger<NoteApiService> logger)
        : base(httpClient, logger)
    {
    }

    public async Task<Result<IEnumerable<NoteDto>>> GetNotesByPatientIdAsync(int patientId)
    {
        return await GetAsync<IEnumerable<NoteDto>>($"api/notes/patient/{patientId}", $"Fetching notes for patient {patientId}");
    }
}
