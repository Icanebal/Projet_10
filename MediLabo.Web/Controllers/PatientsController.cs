using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MediLabo.Web.Services;
using MediLabo.Web.Models.ViewModels;
using MediLabo.Web.Extensions;
using MediLabo.Web.Models;

namespace MediLabo.Web.Controllers;

public class PatientsController : Controller
{
    private readonly PatientService _patientService;
    private readonly AssessmentService _assessmentService;
    private readonly ILogger<PatientsController> _logger;

    public PatientsController(PatientService patientService, ILogger<PatientsController> logger, AssessmentService assessmentService)
    {
        _patientService = patientService;
        _logger = logger;
        _assessmentService = assessmentService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _patientService.GetAllPatientsAsync();

        if (result.IsFailure)
        {
            _logger.LogError("Failed to retrieve patients: {Error}", result.Error);
            TempData.AddToastMessage(new ToastMessage(ToastType.Error, "Erreur lors de la récupération des patients"));
            return View(new List<PatientViewModel>());
        }

        return View(result.Value);
    }

    public async Task<IActionResult> Details(int id)
    {
        var result = await _patientService.GetPatientByIdAsync(id);

        if (result.IsFailure)
        {
            _logger.LogError("Failed to retrieve patient {PatientId}: {Error}", id, result.Error);
            TempData.AddToastMessage(new ToastMessage(ToastType.Error, "Patient introuvable"));
            return RedirectToAction(nameof(Index));
        }

        var riskResult = await _assessmentService.GetDiabetesRiskAsync(id);
        if (riskResult.IsSuccess)
        {
            result.Value!.DiabetesRisk = riskResult.Value;
        }
        else
        {
            _logger.LogWarning("Could not retrieve diabetes risk for patient {PatientId}: {Error}", id, riskResult.Error);
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
        await LoadGendersAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePatientViewModel createPatientViewModel)
    {
        if (!ModelState.IsValid)
        {
            await LoadGendersAsync();
            return View(createPatientViewModel);
        }

        _logger.LogInformation("User creating patient: {FirstName} {LastName}",
            createPatientViewModel.FirstName, createPatientViewModel.LastName);

        var result = await _patientService.CreatePatientAsync(createPatientViewModel);

        if (result.IsFailure)
        {
            _logger.LogError("Failed to create patient: {Error}", result.Error);
            ModelState.AddModelError(string.Empty, "Erreur lors de la création du patient");
            await LoadGendersAsync();
            return View(createPatientViewModel);
        }

        TempData.AddToastMessage(new ToastMessage(ToastType.Success, $"Patient {result.Value?.FullName} créé avec succès !"));
        return RedirectToAction(nameof(Details), new { id = result.Value?.Id });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var result = await _patientService.GetPatientByIdAsync(id);

        if (result.IsFailure)
        {
            _logger.LogError("Failed to retrieve patient {PatientId} for editing: {Error}", id, result.Error);
            TempData.AddToastMessage(new ToastMessage(ToastType.Error, "Patient introuvable"));
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
            ViewData["PatientId"] = id;
            await LoadGendersAsync();
            return View(updatePatientViewModel);
        }

        _logger.LogInformation("User updating patient ID: {PatientId}", id);

        var result = await _patientService.UpdatePatientAsync(id, updatePatientViewModel);

        if (result.IsFailure)
        {
            _logger.LogError("Failed to update patient {PatientId}: {Error}", id, result.Error);
            ModelState.AddModelError(string.Empty, "Erreur lors de la mise à jour du patient");
            ViewData["PatientId"] = id;
            await LoadGendersAsync();
            return View(updatePatientViewModel);
        }

        TempData.AddToastMessage(new ToastMessage(ToastType.Success, $"Patient {result.Value?.FullName} mis à jour avec succès !"));
        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Delete(int id)
    {
        var result = await _patientService.GetPatientByIdAsync(id);

        if (result.IsFailure)
        {
            _logger.LogError("Failed to retrieve patient {PatientId} for deletion: {Error}", id, result.Error);
            TempData.AddToastMessage(new ToastMessage(ToastType.Error, "Patient introuvable"));
            return RedirectToAction(nameof(Index));
        }

        return View(result.Value);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        _logger.LogInformation("User deleting patient ID: {PatientId}", id);

        var result = await _patientService.DeletePatientAsync(id);

        if (result.IsFailure)
        {
            _logger.LogError("Failed to delete patient {PatientId}: {Error}", id, result.Error);
            TempData.AddToastMessage(new ToastMessage(ToastType.Error, "Erreur lors de la suppression du patient"));
            return RedirectToAction(nameof(Index));
        }

        TempData.AddToastMessage(new ToastMessage(ToastType.Success, "Patient supprimé avec succès"));
        return RedirectToAction(nameof(Index));
    }
}
