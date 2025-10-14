using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MediLabo.Web.Services;
using MediLabo.Web.Models.ViewModels;

namespace MediLabo.Web.Controllers;

public class PatientsController : Controller
{
    private readonly PatientService _patientService;
    private readonly ILogger<PatientsController> _logger;

    public PatientsController(PatientService patientService, ILogger<PatientsController> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("Displaying patients list");

        var result = await _patientService.GetAllPatientsAsync();

        if (result.IsFailure)
        {
            _logger.LogError("Failed to retrieve patients: {Error}", result.Error);
            TempData["ErrorMessage"] = result.Error ?? "Erreur lors de la récupération des patients";
            return View(new List<PatientViewModel>());
        }

        return View(result.Value);
    }

    public async Task<IActionResult> Details(int id)
    {
        _logger.LogInformation("Displaying details for patient {PatientId}", id);

        var result = await _patientService.GetPatientByIdAsync(id);

        if (result.IsFailure)
        {
            _logger.LogError("Failed to retrieve patient {PatientId}: {Error}", id, result.Error);
            TempData["ErrorMessage"] = result.Error ?? "Patient introuvable";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Value);
    }

    private async Task LoadGendersAsync()
    {
        var gendersResult = await _patientService.GetAllGendersAsync();
        
        if (gendersResult.IsSuccess && gendersResult.Value != null)
        {
            ViewBag.Genders = new SelectList(gendersResult.Value, "Id", "Name");
        }
        else
        {
            _logger.LogError("Failed to load genders: {Error}", gendersResult.Error);
            ViewBag.Genders = new SelectList(Enumerable.Empty<GenderViewModel>(), "Id", "Name");
        }
    }

    public async Task<IActionResult> Create()
    {
        _logger.LogInformation("Displaying create patient form");
        await LoadGendersAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePatientViewModel createPatientViewModel)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Create patient form validation failed");
            await LoadGendersAsync();
            return View(createPatientViewModel);
        }

        _logger.LogInformation("Creating new patient: {FirstName} {LastName}",
            createPatientViewModel.FirstName,
            createPatientViewModel.LastName);

        var result = await _patientService.CreatePatientAsync(createPatientViewModel);

        if (result.IsFailure)
        {
            _logger.LogError("Failed to create patient: {Error}", result.Error);
            ModelState.AddModelError(string.Empty, result.Error ?? "Erreur lors de la création");
            await LoadGendersAsync();
            return View(createPatientViewModel);
        }

        TempData["SuccessMessage"] = $"Patient {result.Value?.FullName} créé avec succès !";
        return RedirectToAction(nameof(Details), new { id = result.Value?.Id });
    }

    public async Task<IActionResult> Edit(int id)
    {
        _logger.LogInformation("Displaying edit form for patient {PatientId}", id);

        var result = await _patientService.GetPatientByIdAsync(id);

        if (result.IsFailure)
        {
            _logger.LogError("Failed to retrieve patient {PatientId} for editing: {Error}", id, result.Error);
            TempData["ErrorMessage"] = result.Error ?? "Patient introuvable";
            return RedirectToAction(nameof(Index));
        }

        var updateViewModel = new CreatePatientViewModel
        {
            FirstName = result.Value!.FirstName,
            LastName = result.Value.LastName,
            BirthDate = result.Value.BirthDate,
            GenderId = result.Value.GenderId,
            Address = result.Value.Address,
            Phone = result.Value.Phone
        };

        ViewData["PatientId"] = id;
        await LoadGendersAsync();
        return View(updateViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CreatePatientViewModel updatePatientViewModel)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Edit patient form validation failed for patient {PatientId}", id);
            ViewData["PatientId"] = id;
            await LoadGendersAsync();
            return View(updatePatientViewModel);
        }

        _logger.LogInformation("Updating patient {PatientId}", id);

        var result = await _patientService.UpdatePatientAsync(id, updatePatientViewModel);

        if (result.IsFailure)
        {
            _logger.LogError("Failed to update patient {PatientId}: {Error}", id, result.Error);
            ModelState.AddModelError(string.Empty, result.Error ?? "Erreur lors de la modification");
            ViewData["PatientId"] = id;
            await LoadGendersAsync();
            return View(updatePatientViewModel);
        }

        TempData["SuccessMessage"] = $"Patient {result.Value?.FullName} modifié avec succès !";
        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Displaying delete confirmation for patient {PatientId}", id);

        var result = await _patientService.GetPatientByIdAsync(id);

        if (result.IsFailure)
        {
            _logger.LogError("Failed to retrieve patient {PatientId} for deletion: {Error}", id, result.Error);
            TempData["ErrorMessage"] = result.Error ?? "Patient introuvable";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Value);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        _logger.LogInformation("Deleting patient {PatientId}", id);

        var result = await _patientService.DeletePatientAsync(id);

        if (result.IsFailure)
        {
            _logger.LogError("Failed to delete patient {PatientId}: {Error}", id, result.Error);
            TempData["ErrorMessage"] = result.Error ?? "Erreur lors de la suppression";
            return RedirectToAction(nameof(Index));
        }

        TempData["SuccessMessage"] = "Patient supprimé avec succès !";
        return RedirectToAction(nameof(Index));
    }
}
