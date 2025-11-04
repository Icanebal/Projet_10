using System.ComponentModel.DataAnnotations;

namespace MediLabo.Notes.API.Models.DTOs;

public class UpdateNoteDto
{
    [Required(ErrorMessage = "Le contenu de la note est obligatoire")]
    public required string Content { get; set; }
}
