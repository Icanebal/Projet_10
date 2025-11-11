using System.ComponentModel.DataAnnotations;

namespace MediLabo.Web.Models.ViewModels;

public class UpdateNoteViewModel
{
    [Required(ErrorMessage = "Le contenu de la note est obligatoire")]
    [Display(Name = "Contenu de la note")]
    public required string Content { get; set; }
}
