using MediLabo.Common;
using MediLabo.Notes.API.Data;
using MediLabo.Notes.API.Interfaces;
using MediLabo.Notes.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediLabo.Notes.API.Repositories;

public class NoteRepository : INoteRepository
{
    private readonly NotesDbContext _context;

    public NoteRepository(NotesDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IEnumerable<Note>>> GetNotesByPatientIdAsync(int patientId)
    {
        var notes = await _context.Notes
            .Where(n => n.PatientId == patientId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return Result<IEnumerable<Note>>.Success(notes);
    }

    public async Task<Result<Note>> GetNoteByIdAsync(string noteId)
    {
        var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == noteId);

        if (note == null)
        {
            return Result<Note>.Failure("Note not found");
        }

        return Result<Note>.Success(note);
    }

    public async Task<Result<Note>> CreateNoteAsync(Note note)
    {
        await _context.Notes.AddAsync(note);
        await _context.SaveChangesAsync();

        return Result<Note>.Success(note);
    }

    public async Task<Result<Note>> UpdateNoteAsync(string noteId, Note updatedNote)
    {
        var existingNote = await _context.Notes.FirstOrDefaultAsync(n => n.Id == noteId);

        if (existingNote == null)
        {
            return Result<Note>.Failure("Note not found");
        }

        existingNote.Content = updatedNote.Content;
        existingNote.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Result<Note>.Success(existingNote);
    }

    public async Task<Result<bool>> DeleteNoteAsync(string noteId)
    {
        var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == noteId);

        if (note == null)
        {
            return Result<bool>.Failure("Note not found");
        }

        note.IsDeleted = true;
        note.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }
}
