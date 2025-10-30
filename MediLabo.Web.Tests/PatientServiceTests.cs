using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MediLabo.Web.Services;
using MediLabo.Web.Models.ViewModels;
using MediLabo.Common;
using MediLabo.Common.HttpServices;

namespace MediLabo.Web.Tests.Services;

public class PatientServiceTests
{
    private readonly Mock<IApiService> _mockApiService;
    private readonly Mock<ILogger<PatientService>> _mockLogger;
    private readonly PatientService _patientService;

    public PatientServiceTests()
    {
        _mockApiService = new Mock<IApiService>();

        _mockLogger = new Mock<ILogger<PatientService>>();

        _patientService = new PatientService(
            _mockApiService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllPatientsAsync_ReturnsSuccessWithPatients()
    {
        // Arrange
        var patients = new List<PatientViewModel>
        {
            new() { Id = 1, FirstName = "John", LastName = "Doe", BirthDate = new DateTime(1990, 1, 1), GenderName = "Homme" },
            new() { Id = 2, FirstName = "Jane", LastName = "Smith", BirthDate = new DateTime(1985, 5, 15), GenderName = "Femme" }
        };

        _mockApiService
            .Setup(s => s.GetAsync<List<PatientViewModel>>(It.IsAny<string>()))
            .ReturnsAsync(Result<List<PatientViewModel>>.Success(patients));

        // Act
        var result = await _patientService.GetAllPatientsAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().Contain(p => p.FirstName == "John");
    }

    [Fact]
    public async Task GetAllPatientsAsync_WhenApiFails_ReturnsFailure()
    {
        // Arrange
        _mockApiService
            .Setup(s => s.GetAsync<List<PatientViewModel>>(It.IsAny<string>()))
            .ReturnsAsync(Result<List<PatientViewModel>>.Failure("API Error"));

        // Act
        var result = await _patientService.GetAllPatientsAsync();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("API Error");
    }

    [Fact]
    public async Task GetPatientByIdAsync_WithValidId_ReturnsPatient()
    {
        // Arrange
        var patientId = 1;
        var patient = new PatientViewModel
        {
            Id = patientId,
            FirstName = "John",
            LastName = "Doe",
            BirthDate = new DateTime(1990, 1, 1),
            GenderName = "Homme"
        };

        _mockApiService
            .Setup(s => s.GetAsync<PatientViewModel>($"/api/patients/{patientId}"))
            .ReturnsAsync(Result<PatientViewModel>.Success(patient));

        // Act
        var result = await _patientService.GetPatientByIdAsync(patientId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(patientId);
        result.Value.FirstName.Should().Be("John");
    }

    [Fact]
    public async Task CreatePatientAsync_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var createViewModel = new CreatePatientViewModel
        {
            FirstName = "New",
            LastName = "Patient",
            BirthDate = new DateTime(1995, 3, 20),
            GenderId = 2,
        };

        var createdPatient = new PatientViewModel
        {
            Id = 3,
            FirstName = createViewModel.FirstName,
            LastName = createViewModel.LastName,
            BirthDate = createViewModel.BirthDate,
            GenderName = "Femme"
        };

        _mockApiService
            .Setup(s => s.PostAsync<CreatePatientViewModel, PatientViewModel>(
                It.IsAny<string>(),
                It.IsAny<CreatePatientViewModel>()))
            .ReturnsAsync(Result<PatientViewModel>.Success(createdPatient));

        // Act
        var result = await _patientService.CreatePatientAsync(createViewModel);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(3);
    }

    [Fact]
    public async Task DeletePatientAsync_WithValidId_ReturnsSuccess()
    {
        // Arrange
        var patientId = 1;

        _mockApiService
            .Setup(s => s.DeleteAsync($"/api/patients/{patientId}"))
            .ReturnsAsync(Result<bool>.Success(true));

        // Act
        var result = await _patientService.DeletePatientAsync(patientId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }
}
