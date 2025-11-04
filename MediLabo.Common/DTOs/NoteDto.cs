namespace MediLabo.Common.DTOs;

public class NoteDto
{
    public required string Id { get; init; }
    public required int PatientId { get; init; }
    public required string Content { get; init; }
    public required DateTime CreatedAt { get; init; }
}
