using MediLabo.Notes.API.Models.DTOs;
using MediLabo.Notes.API.Models.Entities;
using MediLabo.Common.DTOs;

namespace MediLabo.Notes.API.Utilities;

public static class Mapping
{
    public static NoteDto ToDto(Note note, string patientName)
    {
        return new NoteDto
        {
            Id = note.Id!,
            PatientId = note.PatientId,
            PatientName = patientName,
            Content = note.Content,
            CreatedAt = note.CreatedAt
        };
    }

    public static Note ToEntity(CreateNoteDto createNoteDto)
    {
        return new Note
        {
            PatientId = createNoteDto.PatientId,
            Content = createNoteDto.Content
        };
    }
}
