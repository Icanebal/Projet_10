using FluentAssertions;
using MediLabo.Common;
using MediLabo.Patients.API.Controllers;
using MediLabo.Patients.API.Interfaces;
using MediLabo.Patients.API.Models.DTOs;
using MediLabo.Patients.API.Models.Entities;
using MediLabo.Patients.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace MediLabo.Patients.Tests.Controllers
{
    [Trait("Category", "Unit")]
    public class PatientsControllerTests
    {
        private readonly Mock<IPatientRepository> _mockRepository;
        private readonly Mock<ILogger<PatientService>> _mockLogger;
        private readonly PatientService _service;
        private readonly PatientsController _controller;

        public PatientsControllerTests()
        {
            _mockRepository = new Mock<IPatientRepository>();
            _mockLogger = new Mock<ILogger<PatientService>>();
            _service = new PatientService(_mockRepository.Object, _mockLogger.Object);
            _controller = new PatientsController(_service);
        }

        private Patient CreateTestPatient(int id, string firstName, string lastName, int genderId, string genderName)
        {
            return new Patient
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                BirthDate = new DateTime(1990, 1, 1),
                GenderId = genderId,
                Gender = new Gender { Id = genderId, Name = genderName }
            };
        }

        [Fact]
        public async Task GetPatients_ReturnsOkResultWithPatients()
        {
            var patients = new List<Patient>
            {
                CreateTestPatient(1, "John", "Doe", 1, "Homme")
            };

            _mockRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(patients);

            var result = await _controller.GetPatients();

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);

            var returnedPatients = okResult.Value as IEnumerable<PatientDto>;
            returnedPatients.Should().NotBeNull();
            returnedPatients.Should().HaveCount(1);
            returnedPatients!.First().FirstName.Should().Be("John");
        }

        [Fact]
        public async Task GetPatient_ExistingId_ReturnsOkResultWithPatient()
        {
            var patient = CreateTestPatient(1, "John", "Doe", 1, "Homme");

            _mockRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(patient);

            var result = await _controller.GetPatient(1);

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);

            var returnedPatient = okResult.Value as PatientDto;
            returnedPatient.Should().NotBeNull();
            returnedPatient!.Id.Should().Be(1);
            returnedPatient.FirstName.Should().Be("John");
        }

        [Fact]
        public async Task GetPatient_NonExistingId_ReturnsNotFound()
        {
            _mockRepository
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Patient?)null);

            var result = await _controller.GetPatient(999);

            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult!.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task CreatePatient_ValidData_ReturnsCreatedResult()
        {
            var createDto = new CreatePatientDto
            {
                FirstName = "New",
                LastName = "Patient",
                BirthDate = new DateTime(1995, 3, 20),
                GenderId = 1
            };

            var createdPatient = CreateTestPatient(5, "New", "Patient", 1, "Homme");

            _mockRepository
                .Setup(r => r.GenderExistsAsync(1))
                .ReturnsAsync(true);

            _mockRepository
                .Setup(r => r.CreateAsync(It.IsAny<Patient>()))
                .ReturnsAsync(createdPatient);

            var result = await _controller.CreatePatient(createDto);

            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult!.StatusCode.Should().Be(201);
            createdResult.ActionName.Should().Be("GetPatient");

            var returnedPatient = createdResult.Value as PatientDto;
            returnedPatient.Should().NotBeNull();
            returnedPatient!.Id.Should().Be(5);
            returnedPatient.FirstName.Should().Be("New");
        }

        [Fact]
        public async Task CreatePatient_InvalidModelState_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("FirstName", "Required");

            var createDto = new CreatePatientDto
            {
                FirstName = "",
                LastName = "Patient",
                BirthDate = new DateTime(1995, 3, 20),
                GenderId = 1
            };

            var result = await _controller.CreatePatient(createDto);

            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task UpdatePatient_ExistingId_ReturnsOkResult()
        {
            var updateDto = new CreatePatientDto
            {
                FirstName = "Updated",
                LastName = "Patient",
                BirthDate = new DateTime(1990, 1, 1),
                GenderId = 1
            };

            var existingPatient = CreateTestPatient(1, "Old", "Name", 1, "Homme");

            _mockRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(existingPatient);

            _mockRepository
                .Setup(r => r.GenderExistsAsync(1))
                .ReturnsAsync(true);

            _mockRepository
                .Setup(r => r.UpdateAsync(It.IsAny<Patient>()))
                .ReturnsAsync((Patient p) => p);

            var result = await _controller.UpdatePatient(1, updateDto);

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);

            var returnedPatient = okResult.Value as PatientDto;
            returnedPatient.Should().NotBeNull();
            returnedPatient!.FirstName.Should().Be("Updated");
        }

        [Fact]
        public async Task UpdatePatient_NonExistingId_ReturnsNotFound()
        {
            var updateDto = new CreatePatientDto
            {
                FirstName = "Updated",
                LastName = "Patient",
                BirthDate = new DateTime(1990, 1, 1),
                GenderId = 1
            };

            _mockRepository
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Patient?)null);

            var result = await _controller.UpdatePatient(999, updateDto);

            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult!.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task UpdatePatient_InvalidModelState_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("FirstName", "Required");

            var updateDto = new CreatePatientDto
            {
                FirstName = "",
                LastName = "Patient",
                BirthDate = new DateTime(1990, 1, 1),
                GenderId = 1
            };

            var result = await _controller.UpdatePatient(1, updateDto);

            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task DeletePatient_ExistingId_ReturnsOkWithResult()
        {
            var successResult = Result<bool>.Success(true);
            _mockRepository
                .Setup(r => r.DeleteAsync(1))
                .ReturnsAsync(true);

            var actionResult = await _controller.DeletePatient(1);

            var okResult = actionResult.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);

            var result = okResult.Value as Result<bool>;
            result.Should().NotBeNull();
            result!.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();
        }

        [Fact]
        public async Task DeletePatient_NonExistingId_ReturnsNotFoundWithResult()
        {
            var failureResult = Result<bool>.Failure("Patient with ID 999 not found");
            _mockRepository
               .Setup(r => r.DeleteAsync(999))
               .ReturnsAsync(false);

            var actionResult = await _controller.DeletePatient(999);

            var notFoundResult = actionResult.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult!.StatusCode.Should().Be(404);

            var result = notFoundResult.Value as Result<bool>;
            result.Should().NotBeNull();
            result!.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Patient with ID 999 not found");
        }
    }
}
