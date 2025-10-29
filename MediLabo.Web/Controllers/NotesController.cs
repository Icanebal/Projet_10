using Microsoft.AspNetCore.Mvc;
using MediLabo.Web.Services;
using MediLabo.Web.Models.ViewModels;
using MediLabo.Web.Extensions;
using MediLabo.Web.Models;

namespace MediLabo.Web.Controllers;

public class NotesController : Controller
{
    private readonly NoteService _noteService;
    private readonly PatientService _patientService;

    public NotesController(NoteService noteService, PatientService patientService)
    {
        _noteService = noteService;
        _patientService = patientService;
    }

    [HttpGet]
    public async Task<IActionResult> PatientNotes(int id)
    {
        var patientResult = await _patientService.GetPatientByIdAsync(id);

        if (patientResult.IsFailure)
        {
            TempData.AddToastMessage(new ToastMessage(ToastType.Error, "Patient introuvable"));
            return RedirectToAction("Index", "Patients");
        }

        var notesResult = await _noteService.GetPatientNotesAsync(id);

        if (notesResult.IsFailure)
        {
            TempData.AddToastMessage(new ToastMessage(ToastType.Error, "Erreur lors de la récupération des notes du patient"));
        }

        ViewBag.Patient = patientResult.Value;
        ViewBag.Notes = notesResult.IsSuccess ? notesResult.Value : new List<NoteViewModel>();

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Create(int patientId)
    {
        var patientResult = await _patientService.GetPatientByIdAsync(patientId);

        if (patientResult.IsFailure)
        {
            TempData.AddToastMessage(new ToastMessage(ToastType.Error, "Patient introuvable"));
            return RedirectToAction("Index", "Patients");
        }

        var model = new CreateNoteViewModel
        {
            PatientId = patientId
        };

        ViewBag.Patient = patientResult.Value;

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateNoteViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var patientResult = await _patientService.GetPatientByIdAsync(model.PatientId);
            if (patientResult.IsSuccess)
            {
                ViewBag.Patient = patientResult.Value;
            }
            return View(model);
        }

        var result = await _noteService.CreateNoteAsync(model);

        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, "Erreur lors de la création de la note");
            return View(model);
        }

        TempData.AddToastMessage(new ToastMessage(ToastType.Success, "Note créée avec succès"));
        return RedirectToAction(nameof(PatientNotes), new { id = model.PatientId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var noteResult = await _noteService.GetNoteByIdAsync(id);

        if (noteResult.IsFailure)
        {
            TempData.AddToastMessage(new ToastMessage(ToastType.Error, "Note introuvable"));
            return RedirectToAction("Index", "Patients");
        }

        var note = noteResult.Value!;

        var model = new CreateNoteViewModel
        {
            PatientId = note.PatientId,
            Content = note.Content
        };

        ViewBag.NoteId = id;
        ViewBag.Note = note;

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, CreateNoteViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.NoteId = id;
            return View(model);
        }

        var result = await _noteService.UpdateNoteAsync(id, model);

        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, "Erreur lors de la mise à jour de la note");
            return View(model);
        }

        TempData.AddToastMessage(new ToastMessage(ToastType.Success, "Note mise à jour avec succès"));
        return RedirectToAction(nameof(PatientNotes), new { id = model.PatientId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id, int patientId)
    {
        var result = await _noteService.DeleteNoteAsync(id);

        if (result.IsFailure)
        {
            TempData.AddToastMessage(new ToastMessage(ToastType.Error, "Erreur lors de la suppression de la note"));
        }
        else
        {
            TempData.AddToastMessage(new ToastMessage(ToastType.Success, "Note supprimée avec succès"));
        }

        return RedirectToAction(nameof(PatientNotes), new { id = patientId });
    }
}
