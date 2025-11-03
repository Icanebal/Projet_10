using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediLabo.Common;
using MediLabo.Common.DTOs;
using MediLabo.Notes.API.Models.DTOs;
using MediLabo.Notes.API.Services;

namespace MediLabo.Notes.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly NoteService _noteService;

    public NotesController(NoteService noteService)
    {
        _noteService = noteService;
    }

    [HttpGet("patient/{patientId}")]
    public async Task<ActionResult<Result<IEnumerable<NoteDto>>>> GetNotesByPatientId(int patientId)
    {
        var result = await _noteService.GetNotesByPatientIdAsync(patientId);
        return result.IsFailure ? NotFound(result) : Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Result<NoteDto>>> GetNoteById(string id)
    {
        var result = await _noteService.GetNoteByIdAsync(id);
        return result.IsFailure ? NotFound(result) : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Result<NoteDto>>> CreateNote([FromBody] CreateNoteDto createNoteDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(Result<NoteDto>.Failure("Invalid model state"));

        var result = await _noteService.CreateNoteAsync(createNoteDto);

        if (result.IsFailure)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetNoteById), new { id = result.Value!.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Result<NoteDto>>> UpdateNote(string id, [FromBody] UpdateNoteDto updateNoteDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(Result<NoteDto>.Failure("Invalid model state"));

        var result = await _noteService.UpdateNoteAsync(id, updateNoteDto);
        return result.IsFailure ? NotFound(result) : Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<Result<bool>>> DeleteNote(string id)
    {
        var result = await _noteService.DeleteNoteAsync(id);
        return result.IsFailure ? NotFound(result) : Ok(result);
    }
}
