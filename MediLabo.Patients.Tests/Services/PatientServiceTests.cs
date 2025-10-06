using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MediLabo.Patients.API.Services;
using MediLabo.Patients.API.Interfaces;
using MediLabo.Patients.API.Models.Entities;
using MediLabo.Patients.API.Models.DTOs;

namespace MediLabo.Patients.Tests.Services
{
    [Trait("Category", "Unit")]
    public class PatientServiceTests
    {
        private readonly Mock<IPatientRepository> _mockRepository;
        private readonly Mock<ILogger<PatientService>> _mockLogger;
        private readonly PatientService _service;

        public PatientServiceTests()
        {
            _mockRepository = new Mock<IPatientRepository>();
            _mockLogger = new Mock<ILogger<PatientService>>();
            _service = new PatientService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsListOfPatients()
        {
            var patients = new List<Patient>
            {
                new Patient
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    BirthDate = new DateTime(1990, 1, 1),
                    Gender = "M"
                },
                new Patient
                {
                    Id = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    BirthDate = new DateTime(1985, 5, 15),
                    Gender = "F"
                }
            };

            _mockRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(patients);

            var result = await _service.GetAllAsync();

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().HaveCount(2);
            result.Value.First().FirstName.Should().Be("John");
        }

        [Fact]
        public async Task GetByIdAsync_ExistingPatient_ReturnsPatientDto()
        {
            var patientId = 1;
            var patient = new Patient
            {
                Id = patientId,
                FirstName = "John",
                LastName = "Doe",
                BirthDate = new DateTime(1990, 1, 1),
                Gender = "M",
                Address = "123 Main St",
                Phone = "555-1234"
            };

            _mockRepository
                .Setup(repo => repo.GetByIdAsync(patientId))
                .ReturnsAsync(patient);

            var result = await _service.GetByIdAsync(patientId);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Id.Should().Be(patientId);
            result.Value.FirstName.Should().Be("John");
            result.Value.LastName.Should().Be("Doe");
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingPatient_ReturnsFailure()
        {
            var patientId = 999;
            _mockRepository
                .Setup(repo => repo.GetByIdAsync(patientId))
                .ReturnsAsync((Patient?)null);

            var result = await _service.GetByIdAsync(patientId);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("not found");
            result.Error.Should().Contain(patientId.ToString());
        }

        [Fact]
        public async Task CreateAsync_ValidPatient_ReturnsCreatedPatient()
        {
            var createDto = new CreatePatientDto
            {
                FirstName = "New",
                LastName = "Patient",
                BirthDate = new DateTime(1995, 3, 20),
                Gender = "M",
                Address = "456 Oak Ave",
                Phone = "555-5678"
            };

            var createdPatient = new Patient
            {
                Id = 5,
                FirstName = createDto.FirstName,
                LastName = createDto.LastName,
                BirthDate = createDto.BirthDate,
                Gender = createDto.Gender,
                Address = createDto.Address,
                Phone = createDto.Phone
            };

            _mockRepository
                .Setup(repo => repo.CreateAsync(It.IsAny<Patient>()))
                .ReturnsAsync(createdPatient);

            var result = await _service.CreateAsync(createDto);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Id.Should().Be(5);
            result.Value.FirstName.Should().Be("New");
            result.Value.LastName.Should().Be("Patient");
        }

        [Fact]
        public async Task UpdateAsync_ExistingPatient_ReturnsUpdatedPatient()
        {
            var patientId = 1;
            var existingPatient = new Patient
            {
                Id = patientId,
                FirstName = "Old",
                LastName = "Name",
                BirthDate = new DateTime(1990, 1, 1),
                Gender = "M"
            };

            var updateDto = new CreatePatientDto
            {
                FirstName = "Updated",
                LastName = "Name",
                BirthDate = new DateTime(1990, 1, 1),
                Gender = "M",
                Address = "New Address",
                Phone = "555-9999"
            };

            _mockRepository
                .Setup(repo => repo.GetByIdAsync(patientId))
                .ReturnsAsync(existingPatient);

            _mockRepository
                .Setup(repo => repo.UpdateAsync(It.IsAny<Patient>()))
                .ReturnsAsync((Patient p) => p);

            var result = await _service.UpdateAsync(patientId, updateDto);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.FirstName.Should().Be("Updated");
            result.Value.Address.Should().Be("New Address");
        }

        [Fact]
        public async Task UpdateAsync_NonExistingPatient_ReturnsFailure()
        {
            var patientId = 999;
            var updateDto = new CreatePatientDto
            {
                FirstName = "Updated",
                LastName = "Name",
                BirthDate = new DateTime(1990, 1, 1),
                Gender = "M"
            };

            _mockRepository
                .Setup(repo => repo.GetByIdAsync(patientId))
                .ReturnsAsync((Patient?)null);

            var result = await _service.UpdateAsync(patientId, updateDto);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("not found");
        }

        [Fact]
        public async Task DeleteAsync_ExistingPatient_ReturnsSuccess()
        {
            var patientId = 1;
            _mockRepository
                .Setup(repo => repo.DeleteAsync(patientId))
                .ReturnsAsync(true);

            var result = await _service.DeleteAsync(patientId);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_NonExistingPatient_ReturnsFailure()
        {
            var patientId = 999;
            _mockRepository
                .Setup(repo => repo.DeleteAsync(patientId))
                .ReturnsAsync(false);

            var result = await _service.DeleteAsync(patientId);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("not found");
            result.Error.Should().Contain(patientId.ToString());
        }
    }
}
