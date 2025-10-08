using System.ComponentModel.DataAnnotations;

namespace MediLabo.Web.Models.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Le nom d'utilisateur est obligatoire")]
    [Display(Name = "Nom d'utilisateur")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le mot de passe est obligatoire")]
    [Display(Name = "Mot de passe")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}
