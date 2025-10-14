using Microsoft.AspNetCore.Mvc;
using MediLabo.Web.Services;

namespace MediLabo.Web.Controllers;
public class UsersController : Controller
{
    private readonly UserService _userService;
    private readonly AuthService _authService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        UserService userService,
        AuthService authService,
        ILogger<UsersController> logger)
    {
        _userService = userService;
        _authService = authService;
        _logger = logger;
    }

    private bool CheckAdminAccess()
    {
        if (!_authService.IsAdmin())
        {
            TempData["ErrorMessage"] = "Accès refusé : vous devez être administrateur";
            return false;
        }
        return true;
    }

    public async Task<IActionResult> Index()
    {
        if (!CheckAdminAccess())
            return RedirectToAction("Index", "Home");

        _logger.LogInformation("Displaying users list");

        var result = await _userService.GetAllUsersAsync();

        if (result.IsFailure)
        {
            _logger.LogError("Failed to retrieve users: {Error}", result.Error);
            TempData["ErrorMessage"] = result.Error ?? "Erreur lors de la récupération des utilisateurs";
            return View(new List<MediLabo.Web.Models.ViewModels.UserViewModel>());
        }

        return View(result.Value);
    }

    public async Task<IActionResult> Delete(string id)
    {
        if (!CheckAdminAccess())
            return RedirectToAction("Index", "Home");

        _logger.LogInformation("Displaying delete confirmation for user {UserId}", id);

        var usersResult = await _userService.GetAllUsersAsync();

        if (usersResult.IsFailure)
        {
            TempData["ErrorMessage"] = "Erreur lors de la récupération de l'utilisateur";
            return RedirectToAction(nameof(Index));
        }

        var user = usersResult.Value?.FirstOrDefault(u => u.Id == id);

        if (user == null)
        {
            TempData["ErrorMessage"] = "Utilisateur introuvable";
            return RedirectToAction(nameof(Index));
        }

        return View(user);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        if (!CheckAdminAccess())
            return RedirectToAction("Index", "Home");

        _logger.LogInformation("Deleting user {UserId}", id);

        var result = await _userService.DeleteUserAsync(id);

        if (result.IsFailure)
        {
            _logger.LogError("Failed to delete user {UserId}: {Error}", id, result.Error);
            TempData["ErrorMessage"] = result.Error ?? "Erreur lors de la suppression";
            return RedirectToAction(nameof(Index));
        }

        TempData["SuccessMessage"] = "Utilisateur supprimé avec succès !";
        return RedirectToAction(nameof(Index));
    }
}