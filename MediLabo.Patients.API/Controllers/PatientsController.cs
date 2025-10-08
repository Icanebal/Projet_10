using Microsoft.AspNetCore.Mvc;
using MediLabo.Patients.API.Models.DTOs;
using MediLabo.Patients.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace MediLabo.Patients.API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly PatientService _patientService;

        public PatientsController(PatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetPatients()
        {
            var result = await _patientService.GetAllAsync();
            return result.IsFailure ? BadRequest(result.Error) : Ok(result.Value);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDto>> GetPatient(int id)
        {
            var result = await _patientService.GetByIdAsync(id);
            return result.IsFailure ? NotFound(result.Error) : Ok(result.Value);
        }

        [HttpPost]
        public async Task<ActionResult<PatientDto>> CreatePatient([FromBody] CreatePatientDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _patientService.CreateAsync(createDto);
            return result.IsFailure
                ? BadRequest(result.Error)
                : CreatedAtAction(nameof(GetPatient), new { id = result.Value!.Id }, result.Value);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PatientDto>> UpdatePatient(int id, [FromBody] CreatePatientDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _patientService.UpdateAsync(id, updateDto);
            return result.IsFailure ? NotFound(result.Error) : Ok(result.Value);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var result = await _patientService.DeleteAsync(id);
            return result.IsFailure ? NotFound(result.Error) : NoContent();
        }
    }
}
