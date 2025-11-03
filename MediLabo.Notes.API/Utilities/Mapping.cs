using MediLabo.Notes.API.Models.DTOs;
using MediLabo.Notes.API.Models.Entities;
using MediLabo.Common.DTOs;

namespace MediLabo.Notes.API.Utilities;

public static class Mapping
{
    // Pour GET - Conversion Entity -> DTO
    public static NoteDto ToDto(Note note, string patientName)
    {
        return new NoteDto
        {
            Id = note.Id!,
            PatientId = note.PatientId,
            Content = note.Content,
            CreatedAt = note.CreatedAt
        };
    }

    // Pour CREATE - Conversion CreateDTO -> Entity
    public static Note ToEntityFromCreate(CreateNoteDto createDto)
    {
        return new Note
        {
            PatientId = createDto.PatientId,
            Content = createDto.Content,
            CreatedAt = DateTime.UtcNow
        };
    }

    // Pour UPDATE - Modification en place de l'entité existante
    public static void MapUpdateToEntity(UpdateNoteDto updateDto, Note existingNote)
    {
        existingNote.Content = updateDto.Content;
        existingNote.UpdatedAt = DateTime.UtcNow;
    }
}
