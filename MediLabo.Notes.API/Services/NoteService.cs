using MediLabo.Common;
using MediLabo.Common.HttpServices;
using MediLabo.Notes.API.Interfaces;
using MediLabo.Notes.API.Models.DTOs;
using MediLabo.Notes.API.Utilities;
using MediLabo.Common.DTOs;

namespace MediLabo.Notes.API.Services;

public class NoteService
{
    private readonly INoteRepository _noteRepository;
    private readonly IPatientApiService _patientApiService;
    private readonly ILogger<NoteService> _logger;

    public NoteService(
        INoteRepository noteRepository,
        IPatientApiService patientApiService,
        ILogger<NoteService> logger)
    {
        _noteRepository = noteRepository;
        _patientApiService = patientApiService;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<NoteDto>>> GetNotesByPatientIdAsync(int patientId)
    {
        _logger.LogInformation("Retrieving notes for patient ID: {PatientId}", patientId);

        var patientNameResult = await _patientApiService.GetPatientFullNameAsync(patientId);

        if (!patientNameResult.IsSuccess)
        {
            _logger.LogWarning("Failed to retrieve patient name for ID: {PatientId}", patientId);
            return Result<IEnumerable<NoteDto>>.Failure(patientNameResult.Error!);
        }

        var patientName = patientNameResult.Value!;

        var result = await _noteRepository.GetNotesByPatientIdAsync(patientId);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to retrieve notes for patient ID: {PatientId}. Error: {Error}", patientId, result.Error);
            return Result<IEnumerable<NoteDto>>.Failure(result.Error!);
        }

        var noteDtos = result.Value!.Select(note => Mapping.ToDto(note, patientName));
        _logger.LogInformation("{Count} note(s) retrieved for patient ID: {PatientId}", noteDtos.Count(), patientId);

        return Result<IEnumerable<NoteDto>>.Success(noteDtos);
    }

    public async Task<Result<NoteDto>> GetNoteByIdAsync(string noteId)
    {
        _logger.LogInformation("Retrieving note ID: {NoteId}", noteId);

        var result = await _noteRepository.GetNoteByIdAsync(noteId);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to retrieve note ID: {NoteId}. Error: {Error}", noteId, result.Error);
            return Result<NoteDto>.Failure(result.Error!);
        }

        var note = result.Value!;

        var patientNameResult = await _patientApiService.GetPatientFullNameAsync(note.PatientId);

        if (!patientNameResult.IsSuccess)
        {
            _logger.LogWarning("Failed to retrieve patient name for patient ID: {PatientId}", note.PatientId);
            return Result<NoteDto>.Failure(patientNameResult.Error!);
        }

        var noteDto = Mapping.ToDto(note, patientNameResult.Value!);
        _logger.LogInformation("Note ID: {NoteId} retrieved successfully", noteId);

        return Result<NoteDto>.Success(noteDto);
    }

    public async Task<Result<NoteDto>> CreateNoteAsync(CreateNoteDto createNoteDto)
    {
        _logger.LogInformation("Creating note for patient ID: {PatientId}", createNoteDto.PatientId);

        var patientNameResult = await _patientApiService.GetPatientFullNameAsync(createNoteDto.PatientId);

        if (!patientNameResult.IsSuccess)
        {
            _logger.LogWarning("Failed to retrieve patient for ID: {PatientId}", createNoteDto.PatientId);
            return Result<NoteDto>.Failure(patientNameResult.Error!);
        }

        var note = Mapping.ToEntity(createNoteDto);
        var result = await _noteRepository.CreateNoteAsync(note);

        if (!result.IsSuccess)
        {
            _logger.LogError("Failed to create note for patient ID: {PatientId}. Error: {Error}", createNoteDto.PatientId, result.Error);
            return Result<NoteDto>.Failure(result.Error!);
        }

        var noteDto = Mapping.ToDto(result.Value!, patientNameResult.Value!);
        _logger.LogInformation("Note created successfully - ID: {NoteId} for patient ID: {PatientId}", noteDto.Id, createNoteDto.PatientId);

        return Result<NoteDto>.Success(noteDto);
    }

    public async Task<Result<NoteDto>> UpdateNoteAsync(string noteId, CreateNoteDto updateNoteDto)
    {
        _logger.LogInformation("Updating note ID: {NoteId}", noteId);

        var patientNameResult = await _patientApiService.GetPatientFullNameAsync(updateNoteDto.PatientId);

        if (!patientNameResult.IsSuccess)
        {
            _logger.LogWarning("Failed to retrieve patient for ID: {PatientId}", updateNoteDto.PatientId);
            return Result<NoteDto>.Failure(patientNameResult.Error!);
        }

        var noteToUpdate = Mapping.ToEntity(updateNoteDto);
        var result = await _noteRepository.UpdateNoteAsync(noteId, noteToUpdate);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to update note ID: {NoteId}. Error: {Error}", noteId, result.Error);
            return Result<NoteDto>.Failure(result.Error!);
        }

        var noteDto = Mapping.ToDto(result.Value!, patientNameResult.Value!);
        _logger.LogInformation("Note ID: {NoteId} updated successfully", noteId);

        return Result<NoteDto>.Success(noteDto);
    }

    public async Task<Result<bool>> DeleteNoteAsync(string noteId)
    {
        _logger.LogInformation("Deleting note ID: {NoteId}", noteId);

        var result = await _noteRepository.DeleteNoteAsync(noteId);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to delete note ID: {NoteId}. Error: {Error}", noteId, result.Error);
            return Result<bool>.Failure(result.Error!);
        }

        _logger.LogInformation("Note ID: {NoteId} deleted successfully", noteId);
        return Result<bool>.Success(true);
    }
}