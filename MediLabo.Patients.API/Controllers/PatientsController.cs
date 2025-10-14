using Microsoft.AspNetCore.Mvc;
using MediLabo.Patients.API.Models.DTOs;
using MediLabo.Patients.API.Services;
using MediLabo.Common;
using Microsoft.AspNetCore.Authorization;

namespace MediLabo.Patients.API.Controllers
{
    [Authorize]
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
        public async Task<ActionResult<Result<IEnumerable<PatientDto>>>> GetPatients()
        {
            var result = await _patientService.GetAllAsync();
            return result.IsFailure ? BadRequest(result) : Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<PatientDto>>> GetPatient(int id)
        {
            var result = await _patientService.GetByIdAsync(id);
            return result.IsFailure ? NotFound(result) : Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Result<PatientDto>>> CreatePatient([FromBody] CreatePatientDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(Result<PatientDto>.Failure("Invalid model state"));

            var result = await _patientService.CreateAsync(createDto);

            if (result.IsFailure)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetPatient), new { id = result.Value!.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<PatientDto>>> UpdatePatient(int id, [FromBody] CreatePatientDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(Result<PatientDto>.Failure("Invalid model state"));

            var result = await _patientService.UpdateAsync(id, updateDto);
            return result.IsFailure ? NotFound(result) : Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> DeletePatient(int id)
        {
            var result = await _patientService.DeleteAsync(id);
            return result.IsFailure ? NotFound(result) : Ok(result);
        }

        [Authorize]
        [HttpGet("genders")]
        public async Task<ActionResult<Result<IEnumerable<GenderDto>>>> GetGenders()
        {
            var result = await _patientService.GetAllGendersAsync();
            return result.IsFailure ? BadRequest(result) : Ok(result);
        }
    }
}