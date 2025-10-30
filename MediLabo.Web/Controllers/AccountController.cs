using Microsoft.AspNetCore.Mvc;
using MediLabo.Web.Services;
using MediLabo.Web.Models.ViewModels;
using MediLabo.Web.Extensions;
using MediLabo.Web.Models;

namespace MediLabo.Web.Controllers;

public class AccountController : Controller
{
    private readonly AuthService _authService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(AuthService authService, ILogger<AccountController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (_authService.IsAuthenticated())
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(loginViewModel);
        }

        _logger.LogInformation("Login attempt for user {Username}", loginViewModel.Username);

        var result = await _authService.LoginAsync(loginViewModel.Username, loginViewModel.Password);

        if (result.IsFailure)
        {
            _logger.LogWarning("Login failed for user {Username}", loginViewModel.Username);
            ModelState.AddModelError(string.Empty, result.Error ?? "Erreur lors de la connexion");
            return View(loginViewModel);
        }

        _logger.LogInformation("User {Username} logged in successfully", loginViewModel.Username);
        TempData.AddToastMessage(new ToastMessage(ToastType.Success, "Connexion réussie"));

        if (!string.IsNullOrEmpty(loginViewModel.ReturnUrl) && Url.IsLocalUrl(loginViewModel.ReturnUrl))
        {
            return Redirect(loginViewModel.ReturnUrl);
        }

        return RedirectToAction("Index", "Patients");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        var username = _authService.GetUserName();
        _logger.LogInformation("User {Username} logging out", username);

        _authService.Logout();

        TempData.AddToastMessage(new ToastMessage(ToastType.Success, "Déconnexion réussie"));
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (!_authService.IsAuthenticated())
        {
            TempData.AddToastMessage(new ToastMessage(ToastType.Error, "Vous devez être connecté pour accéder à cette page"));
            return RedirectToAction("Login");
        }

        if (!_authService.IsAdmin())
        {
            TempData.AddToastMessage(new ToastMessage(ToastType.Error, "Accès refusé : vous devez être administrateur"));
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
    {
        if (!_authService.IsAuthenticated())
        {
            TempData.AddToastMessage(new ToastMessage(ToastType.Error, "Vous devez être connecté pour accéder à cette page"));
            return RedirectToAction("Login");
        }

        if (!_authService.IsAdmin())
        {
            TempData.AddToastMessage(new ToastMessage(ToastType.Error, "Accès refusé : vous devez être administrateur"));
            return RedirectToAction("Index", "Home");
        }

        if (!ModelState.IsValid)
        {
            return View(registerViewModel);
        }

        _logger.LogInformation("Registration attempt for email {Email}", registerViewModel.Email);

        var result = await _authService.RegisterAsync(
            registerViewModel.Email,
            registerViewModel.Password,
            registerViewModel.FirstName,
            registerViewModel.LastName,
            registerViewModel.Role);

        if (result.IsFailure)
        {
            _logger.LogWarning("Registration failed for email {Email}", registerViewModel.Email);
            ModelState.AddModelError(string.Empty, result.Error ?? "Erreur inconnue");
            return View(registerViewModel);
        }

        _logger.LogInformation("User {Email} registered successfully", registerViewModel.Email);
        TempData.AddToastMessage(new ToastMessage(ToastType.Success, "Utilisateur créé avec succès"));
        return RedirectToAction("Index", "Users");
    }
}
