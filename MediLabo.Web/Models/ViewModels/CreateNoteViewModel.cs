using System.ComponentModel.DataAnnotations;

namespace MediLabo.Web.Models.ViewModels;

public class CreateNoteViewModel
{
    public required int PatientId { get; set; }

    [Required(ErrorMessage = "Le contenu de la note est obligatoire")]
    public string Content { get; set; } = string.Empty;
}