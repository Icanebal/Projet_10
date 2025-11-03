using MediLabo.Common;
using MediLabo.Notes.API.Interfaces;
using MediLabo.Notes.API.Models.DTOs;
using MediLabo.Notes.API.Utilities;
using MediLabo.Common.DTOs;

namespace MediLabo.Notes.API.Services;

public class NoteService
{
    private readonly INoteRepository _noteRepository;
    private readonly IPatientService _patientService;
    private readonly ILogger<NoteService> _logger;

    public NoteService(INoteRepository noteRepository, IPatientService patientService, ILogger<NoteService> logger)
    {
        _noteRepository = noteRepository;
        _patientService = patientService;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<NoteDto>>> GetNotesByPatientIdAsync(int patientId)
    {
        _logger.LogInformation("Retrieving notes for patient ID: {PatientId}", patientId);

        var patientResult = await _patientService.GetPatientAsync(patientId);

        if (!patientResult.IsSuccess)
        {
            _logger.LogWarning("Failed to retrieve patient for ID: {PatientId}", patientId);
            return Result<IEnumerable<NoteDto>>.Failure(patientResult.Error!);
        }

        var patientName = patientResult.Value!.FullName;

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

        var patientResult = await _patientService.GetPatientAsync(note.PatientId);

        if (!patientResult.IsSuccess)
        {
            _logger.LogWarning("Failed to retrieve patient for patient ID: {PatientId}", note.PatientId);
            return Result<NoteDto>.Failure(patientResult.Error!);
        }

        var noteDto = Mapping.ToDto(note, patientResult.Value!.FullName);
        _logger.LogInformation("Note ID: {NoteId} retrieved successfully", noteId);

        return Result<NoteDto>.Success(noteDto);
    }

    public async Task<Result<NoteDto>> CreateNoteAsync(CreateNoteDto createNoteDto)
    {
        _logger.LogInformation("Creating note for patient ID: {PatientId}", createNoteDto.PatientId);

        var patientResult = await _patientService.GetPatientAsync(createNoteDto.PatientId);

        if (!patientResult.IsSuccess)
        {
            _logger.LogWarning("Failed to retrieve patient for ID: {PatientId}", createNoteDto.PatientId);
            return Result<NoteDto>.Failure(patientResult.Error!);
        }

        var note = Mapping.ToEntityFromCreate(createNoteDto);
        var result = await _noteRepository.CreateNoteAsync(note);

        if (!result.IsSuccess)
        {
            _logger.LogError("Failed to create note for patient ID: {PatientId}. Error: {Error}", createNoteDto.PatientId, result.Error);
            return Result<NoteDto>.Failure(result.Error!);
        }

        var noteDto = Mapping.ToDto(result.Value!, patientResult.Value!.FullName);
        _logger.LogInformation("Note created successfully - ID: {NoteId} for patient ID: {PatientId}", noteDto.Id, createNoteDto.PatientId);

        return Result<NoteDto>.Success(noteDto);
    }

    public async Task<Result<NoteDto>> UpdateNoteAsync(string noteId, UpdateNoteDto updateNoteDto)
    {
        _logger.LogInformation("Updating note ID: {NoteId}", noteId);

        var existingNoteResult = await _noteRepository.GetNoteByIdAsync(noteId);

        if (!existingNoteResult.IsSuccess)
        {
            _logger.LogWarning("Failed to retrieve note ID: {NoteId}. Error: {Error}", noteId, existingNoteResult.Error);
            return Result<NoteDto>.Failure(existingNoteResult.Error!);
        }

        var existingNote = existingNoteResult.Value!;

        var patientResult = await _patientService.GetPatientAsync(existingNote.PatientId);

        if (!patientResult.IsSuccess)
        {
            _logger.LogWarning("Failed to retrieve patient for ID: {PatientId}", existingNote.PatientId);
            return Result<NoteDto>.Failure(patientResult.Error!);
        }

        Mapping.MapUpdateToEntity(updateNoteDto, existingNote);
        
        var updateResult = await _noteRepository.UpdateNoteAsync(noteId, existingNote);

        if (!updateResult.IsSuccess)
        {
            _logger.LogWarning("Failed to update note ID: {NoteId}. Error: {Error}", noteId, updateResult.Error);
            return Result<NoteDto>.Failure(updateResult.Error!);
        }

        var noteDto = Mapping.ToDto(updateResult.Value!, patientResult.Value!.FullName);
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