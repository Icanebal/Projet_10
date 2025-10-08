using System.ComponentModel.DataAnnotations;

namespace MediLabo.Web.Models.ViewModels;

public class CreatePatientViewModel
{
    [Required(ErrorMessage = "Le prénom est obligatoire")]
    [Display(Name = "Prénom")]
    [StringLength(100, ErrorMessage = "Le prénom ne peut pas dépasser 100 caractères")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le nom est obligatoire")]
    [Display(Name = "Nom")]
    [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "La date de naissance est obligatoire")]
    [Display(Name = "Date de naissance")]
    [DataType(DataType.Date)]
    public DateTime BirthDate { get; set; } = DateTime.Today.AddYears(-30);

    [Required(ErrorMessage = "Le genre est obligatoire")]
    [Display(Name = "Genre")]
    public string Gender { get; set; } = string.Empty;

    [Display(Name = "Adresse")]
    [StringLength(200, ErrorMessage = "L'adresse ne peut pas dépasser 200 caractères")]
    public string? Address { get; set; }

    [Display(Name = "Téléphone")]
    [Phone(ErrorMessage = "Le numéro de téléphone n'est pas valide")]
    [StringLength(20, ErrorMessage = "Le numéro de téléphone ne peut pas dépasser 20 caractères")]
    public string? Phone { get; set; }
}