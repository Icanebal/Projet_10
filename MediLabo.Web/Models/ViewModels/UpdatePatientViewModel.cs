using System.ComponentModel.DataAnnotations;

namespace MediLabo.Web.Models.ViewModels;

public class UpdatePatientViewModel
{
    [Required(ErrorMessage = "Le prénom est obligatoire")]
    [Display(Name = "Prénom")]
    [StringLength(100, ErrorMessage = "Le prénom ne peut pas dépasser 100 caractères")]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "Le nom est obligatoire")]
    [Display(Name = "Nom")]
    [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
    public required string LastName { get; set; }

    [Required(ErrorMessage = "La date de naissance est obligatoire")]
    [Display(Name = "Date de naissance")]
    [DataType(DataType.Date)]
    public DateTime BirthDate { get; set; }

    [Required(ErrorMessage = "Le genre est obligatoire")]
    [Display(Name = "Genre")]
    [Range(1, int.MaxValue, ErrorMessage = "Veuillez sélectionner un genre")]
    public required int GenderId { get; set; }

    [Display(Name = "Adresse")]
    [StringLength(200, ErrorMessage = "L'adresse ne peut pas dépasser 200 caractères")]
    public string? Address { get; set; }

    [Display(Name = "Téléphone")]
    [Phone(ErrorMessage = "Le numéro de téléphone n'est pas valide")]
    [StringLength(20, ErrorMessage = "Le numéro de téléphone ne peut pas dépasser 20 caractères")]
    public string? Phone { get; set; }
}
