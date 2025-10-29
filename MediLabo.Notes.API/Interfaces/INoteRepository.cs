using MediLabo.Common;
using MediLabo.Notes.API.Models.Entities;

namespace MediLabo.Notes.API.Interfaces;

public interface INoteRepository
{
    Task<Result<IEnumerable<Note>>> GetNotesByPatientIdAsync(int patientId);
    Task<Result<Note>> GetNoteByIdAsync(string noteId);
    Task<Result<Note>> CreateNoteAsync(Note note);
    Task<Result<Note>> UpdateNoteAsync(string noteId, Note note);
    Task<Result<bool>> DeleteNoteAsync(string noteId);
}
