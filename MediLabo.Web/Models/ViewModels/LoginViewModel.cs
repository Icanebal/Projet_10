using System.ComponentModel.DataAnnotations;

namespace MediLabo.Web.Models.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Le nom d'utilisateur est obligatoire")]
    [Display(Name = "Nom d'utilisateur")]
    public required string Username { get; set; }

    [Required(ErrorMessage = "Le mot de passe est obligatoire")]
    [Display(Name = "Mot de passe")]
    [DataType(DataType.Password)]
    public required string Password { get; set; }

    public string? ReturnUrl { get; set; }
}
