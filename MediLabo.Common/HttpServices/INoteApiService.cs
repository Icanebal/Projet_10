using MediLabo.Common.DTOs;

namespace MediLabo.Common.HttpServices;

public interface INoteApiService
{
    Task<Result<IEnumerable<NoteDto>>> GetNotesByPatientIdAsync(int patientId);
}
