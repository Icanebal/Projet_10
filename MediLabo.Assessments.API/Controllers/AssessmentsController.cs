using MediLabo.Assessments.API.Services;
using MediLabo.Common;
using MediLabo.Common.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediLabo.Assessments.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AssessmentsController : ControllerBase
{
    private readonly DiabetesRiskService _diabetesRiskService;
    private readonly ILogger<AssessmentsController> _logger;

    public AssessmentsController(DiabetesRiskService diabetesRiskService, ILogger<AssessmentsController> logger)
    {
        _diabetesRiskService = diabetesRiskService;
        _logger = logger;
    }

    [HttpGet("diabetes/{patientId}")]
    public async Task<ActionResult<Result<DiabetesRiskResponse>>> GetDiabetesRisk(int patientId)
    {
        _logger.LogInformation("Received request to calculate diabetes risk for patient {PatientId}", patientId);

        var result = await _diabetesRiskService.CalculateRiskAsync(patientId);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to calculate risk for patient {PatientId}: {Error}", patientId, result.Error);
            return NotFound(result);
        }

        _logger.LogInformation("Successfully calculated risk for patient {PatientId}: {RiskLevel}",
            patientId, result.Value!.RiskLevel);

        return Ok(result);
    }
}