using MediLabo.Assessments.API.Calculators;
using MediLabo.Assessments.API.Models;
using MediLabo.Assessments.API.Services;
using MediLabo.Common;
using MediLabo.Common.DTOs;
using MediLabo.Common.HttpServices;
using MediLabo.Common.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace MediLabo.Assessments.API.Tests.Services;

public class DiabetesRiskServiceTests
{
    private readonly Mock<IApiService> _mockApiService;
    private readonly AgeCalculator _ageCalculator;
    private readonly TriggerTermsCalculator _triggerTermsCalculator;
    private readonly DiabetesRiskCalculator _diabetesRiskCalculator;
    private readonly Mock<ILogger<DiabetesRiskService>> _mockLogger;
    private readonly DiabetesRiskService _service;

    public DiabetesRiskServiceTests()
    {
        _mockApiService = new Mock<IApiService>();
        _ageCalculator = new AgeCalculator();
        _triggerTermsCalculator = new TriggerTermsCalculator();
        _diabetesRiskCalculator = new DiabetesRiskCalculator();
        _mockLogger = new Mock<ILogger<DiabetesRiskService>>();

        _service = new DiabetesRiskService(
            _mockApiService.Object,
            _ageCalculator,
            _triggerTermsCalculator,
            _diabetesRiskCalculator,
            _mockLogger.Object);
    }

    [Fact]
    public async Task CalculateRiskAsync_WhenSuccessful_ReturnsNoneRisk()
    {
        // Arrange
        var patientId = 1;
        var patient = new PatientAssessmentDto
        {
            Id = patientId,
            BirthDate = new DateTime(1966, 12, 31),
            GenderId = 2
        };

        var notes = new List<NoteDto>
        {
            new () { Id = "1", PatientId = patientId, Content = "Le patient se sent bien. Poids normal.", CreatedAt = DateTime.UtcNow }
        };

        _mockApiService
            .Setup(x => x.GetAsync<PatientAssessmentDto>($"api/patients/{patientId}"))
            .ReturnsAsync(Result<PatientAssessmentDto>.Success(patient));

        _mockApiService
            .Setup(x => x.GetAsync<IEnumerable<NoteDto>>($"api/notes/patient/{patientId}"))
            .ReturnsAsync(Result<IEnumerable<NoteDto>>.Success(notes));

        // Act
        var result = await _service.CalculateRiskAsync(patientId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(patientId, result.Value.PatientId);
        Assert.Equal(DiabetesRiskLevel.None, result.Value.RiskLevel);
    }

    [Fact]
    public async Task CalculateRiskAsync_WithBorderlineRisk_ReturnsBorderline()
    {
        // Arrange
        var patientId = 2;
        var patient = new PatientAssessmentDto
        {
            Id = patientId,
            BirthDate = new DateTime(1945, 6, 24),
            GenderId = 1
        };

        var notes = new List<NoteDto>
        {
            new NoteDto
            {
                Id = "1",
                PatientId = patientId,
                Content = "Stress et audition anormale",
                CreatedAt = DateTime.UtcNow
            },
            new NoteDto
            {
                Id = "2",
                PatientId = patientId,
                Content = "Réaction aux médicaments",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };

        _mockApiService
            .Setup(x => x.GetAsync<PatientAssessmentDto>($"api/patients/{patientId}"))
            .ReturnsAsync(Result<PatientAssessmentDto>.Success(patient));

        _mockApiService
            .Setup(x => x.GetAsync<IEnumerable<NoteDto>>($"api/notes/patient/{patientId}"))
            .ReturnsAsync(Result<IEnumerable<NoteDto>>.Success(notes));

        // Act
        var result = await _service.CalculateRiskAsync(patientId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(DiabetesRiskLevel.Borderline, result.Value!.RiskLevel);
    }

    [Fact]
    public async Task CalculateRiskAsync_WithInDangerRisk_ReturnsInDanger()
    {
        // Arrange
        var patientId = 3;
        var patient = new PatientAssessmentDto
        {
            Id = patientId,
            BirthDate = new DateTime(2004, 6, 18),
            GenderId = 1
        };

        var notes = new List<NoteDto>
        {
            new NoteDto
            {
                Id = "1",
                PatientId = patientId,
                Content = "Patient fumeur",
                CreatedAt = DateTime.UtcNow
            },
            new NoteDto
            {
                Id = "2",
                PatientId = patientId,
                Content = "Cholestérol élevé, audition anormale",
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            }
        };

        _mockApiService
            .Setup(x => x.GetAsync<PatientAssessmentDto>($"api/patients/{patientId}"))
            .ReturnsAsync(Result<PatientAssessmentDto>.Success(patient));

        _mockApiService
            .Setup(x => x.GetAsync<IEnumerable<NoteDto>>($"api/notes/patient/{patientId}"))
            .ReturnsAsync(Result<IEnumerable<NoteDto>>.Success(notes));

        // Act
        var result = await _service.CalculateRiskAsync(patientId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(DiabetesRiskLevel.InDanger, result.Value!.RiskLevel);
    }

    [Fact]
    public async Task CalculateRiskAsync_WithEarlyOnsetRisk_ReturnsEarlyOnset()
    {
        // Arrange
        var patientId = 4;
        var patient = new PatientAssessmentDto
        {
            Id = patientId,
            BirthDate = new DateTime(2002, 6, 28),
            GenderId = 2
        };

        var notes = new List<NoteDto>
        {
            new NoteDto
            {
                Id = "1",
                PatientId = patientId,
                Content = "Anticorps élevés, Réaction",
                CreatedAt = DateTime.UtcNow
            },
            new NoteDto
            {
                Id = "2",
                PatientId = patientId,
                Content = "Fumeur, Hémoglobine A1C élevée",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new NoteDto
            {
                Id = "3",
                PatientId = patientId,
                Content = "Taille, Poids, Cholestérol élevé",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new NoteDto
            {
                Id = "4",
                PatientId = patientId,
                Content = "Vertige et Réaction aux médicaments",
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            }
        };

        _mockApiService
            .Setup(x => x.GetAsync<PatientAssessmentDto>($"api/patients/{patientId}"))
            .ReturnsAsync(Result<PatientAssessmentDto>.Success(patient));

        _mockApiService
            .Setup(x => x.GetAsync<IEnumerable<NoteDto>>($"api/notes/patient/{patientId}"))
            .ReturnsAsync(Result<IEnumerable<NoteDto>>.Success(notes));

        // Act
        var result = await _service.CalculateRiskAsync(patientId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(DiabetesRiskLevel.EarlyOnset, result.Value!.RiskLevel);
    }

    [Fact]
    public async Task CalculateRiskAsync_WhenPatientNotFound_ReturnsFailure()
    {
        // Arrange
        var patientId = 999;
        var errorMessage = "Patient not found";

        _mockApiService
            .Setup(x => x.GetAsync<PatientAssessmentDto>($"api/patients/{patientId}"))
            .ReturnsAsync(Result<PatientAssessmentDto>.Failure(errorMessage));

        // Act
        var result = await _service.CalculateRiskAsync(patientId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(errorMessage, result.Error);
        Assert.Null(result.Value);

        _mockApiService.Verify(x => x.GetAsync<IEnumerable<NoteDto>>(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task CalculateRiskAsync_WhenNotesNotFound_ReturnsFailure()
    {
        // Arrange
        var patientId = 1;
        var errorMessage = "Notes not found";
        var patient = new PatientAssessmentDto
        {
            Id = patientId,
            BirthDate = new DateTime(1990, 1, 1),
            GenderId = 1
        };

        _mockApiService
            .Setup(x => x.GetAsync<PatientAssessmentDto>($"api/patients/{patientId}"))
            .ReturnsAsync(Result<PatientAssessmentDto>.Success(patient));

        _mockApiService
            .Setup(x => x.GetAsync<IEnumerable<NoteDto>>($"api/notes/patient/{patientId}"))
            .ReturnsAsync(Result<IEnumerable<NoteDto>>.Failure(errorMessage));

        // Act
        var result = await _service.CalculateRiskAsync(patientId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(errorMessage, result.Error);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task CalculateRiskAsync_WithNoNotes_ReturnsNoneRisk()
    {
        // Arrange
        var patientId = 1;
        var patient = new PatientAssessmentDto
        {
            Id = patientId,
            BirthDate = new DateTime(1980, 1, 1),
            GenderId = 1
        };

        var emptyNotes = new List<NoteDto>();

        _mockApiService
            .Setup(x => x.GetAsync<PatientAssessmentDto>($"api/patients/{patientId}"))
            .ReturnsAsync(Result<PatientAssessmentDto>.Success(patient));

        _mockApiService
            .Setup(x => x.GetAsync<IEnumerable<NoteDto>>($"api/notes/patient/{patientId}"))
            .ReturnsAsync(Result<IEnumerable<NoteDto>>.Success(emptyNotes));

        // Act
        var result = await _service.CalculateRiskAsync(patientId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(DiabetesRiskLevel.None, result.Value!.RiskLevel);
        Assert.Equal(0, result.Value.TriggerCount);
    }

    [Fact]
    public async Task CalculateRiskAsync_WithMalePatient_MapsGenderCorrectly()
    {
        // Arrange
        var patientId = 1;
        var patient = new PatientAssessmentDto
        {
            Id = patientId,
            BirthDate = new DateTime(1990, 1, 1),
            GenderId = 1
        };

        var notes = new List<NoteDto>
        {
            new NoteDto
            {
                Id = "1",
                PatientId = patientId,
                Content = "Note",
                CreatedAt = DateTime.UtcNow
            }
        };

        _mockApiService
            .Setup(x => x.GetAsync<PatientAssessmentDto>($"api/patients/{patientId}"))
            .ReturnsAsync(Result<PatientAssessmentDto>.Success(patient));

        _mockApiService
            .Setup(x => x.GetAsync<IEnumerable<NoteDto>>($"api/notes/patient/{patientId}"))
            .ReturnsAsync(Result<IEnumerable<NoteDto>>.Success(notes));

        // Act
        var result = await _service.CalculateRiskAsync(patientId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(Gender.Male, result.Value!.Gender);
    }

    [Fact]
    public async Task CalculateRiskAsync_WithFemalePatient_MapsGenderCorrectly()
    {
        // Arrange
        var patientId = 1;
        var patient = new PatientAssessmentDto
        {
            Id = patientId,
            BirthDate = new DateTime(1990, 1, 1),
            GenderId = 2
        };

        var notes = new List<NoteDto>
        {
            new NoteDto
            {
                Id = "1",
                PatientId = patientId,
                Content = "Note",
                CreatedAt = DateTime.UtcNow
            }
        };

        _mockApiService
            .Setup(x => x.GetAsync<PatientAssessmentDto>($"api/patients/{patientId}"))
            .ReturnsAsync(Result<PatientAssessmentDto>.Success(patient));

        _mockApiService
            .Setup(x => x.GetAsync<IEnumerable<NoteDto>>($"api/notes/patient/{patientId}"))
            .ReturnsAsync(Result<IEnumerable<NoteDto>>.Success(notes));

        // Act
        var result = await _service.CalculateRiskAsync(patientId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(Gender.Female, result.Value!.Gender);
    }
}

