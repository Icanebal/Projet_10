using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MediLabo.Patients.API.Services;
using MediLabo.Patients.API.Interfaces;
using MediLabo.Patients.API.Models.Entities;
using MediLabo.Patients.API.Models.DTOs;
using MediLabo.Common;

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
        public async Task GetAllAsync_ReturnsSuccessWithListOfPatients()
        {
            var patients = new List<Patient>
            {
                CreateTestPatient(1, "John", "Doe", 1, "Homme"),
                CreateTestPatient(2, "Jane", "Smith", 2, "Femme")
            };

            _mockRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(Result<IEnumerable<Patient>>.Success(patients));

            var result = await _service.GetAllAsync();

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().HaveCount(2);
            result.Value.First().FirstName.Should().Be("John");
        }

        [Fact]
        public async Task GetByIdAsync_ExistingPatient_ReturnsSuccessWithPatientDto()
        {
            var patientId = 1;
            var patient = CreateTestPatient(patientId, "John", "Doe", 1, "Homme");
            patient.Address = "123 Main St";
            patient.Phone = "555-1234";

            _mockRepository
                .Setup(repo => repo.GetByIdAsync(patientId))
                .ReturnsAsync(Result<Patient>.Success(patient));

            var result = await _service.GetByIdAsync(patientId);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Id.Should().Be(patientId);
            result.Value.FirstName.Should().Be("John");
            result.Value.LastName.Should().Be("Doe");
            result.Value.GenderName.Should().Be("Homme");
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingPatient_ReturnsFailure()
        {
            var patientId = 999;
            _mockRepository
                .Setup(repo => repo.GetByIdAsync(patientId))
                .ReturnsAsync(Result<Patient>.Failure("Patient not found"));

            var result = await _service.GetByIdAsync(patientId);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Patient not found");
        }

        [Fact]
        public async Task CreateAsync_ValidPatient_ReturnsSuccessWithCreatedPatient()
        {
            var createDto = new CreatePatientDto
            {
                FirstName = "New",
                LastName = "Patient",
                BirthDate = new DateTime(1995, 3, 20),
                GenderId = 1,
                Address = "456 Oak Ave",
                Phone = "555-5678"
            };

            var createdPatient = CreateTestPatient(5, "New", "Patient", 1, "Homme");
            createdPatient.Address = createDto.Address;
            createdPatient.Phone = createDto.Phone;

            _mockRepository
                .Setup(repo => repo.GenderExistsAsync(1))
                .ReturnsAsync(Result<bool>.Success(true));

            _mockRepository
                .Setup(repo => repo.CreateAsync(It.IsAny<Patient>()))
                .ReturnsAsync(Result<Patient>.Success(createdPatient));

            var result = await _service.CreateAsync(createDto);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Id.Should().Be(5);
            result.Value.FirstName.Should().Be("New");
            result.Value.LastName.Should().Be("Patient");
        }

        [Fact]
        public async Task CreateAsync_InvalidGenderId_ReturnsFailure()
        {
            var createDto = new CreatePatientDto
            {
                FirstName = "New",
                LastName = "Patient",
                BirthDate = new DateTime(1995, 3, 20),
                GenderId = 999
            };

            _mockRepository
                .Setup(repo => repo.GenderExistsAsync(999))
                .ReturnsAsync(Result<bool>.Failure("Gender not found"));

            var result = await _service.CreateAsync(createDto);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Gender not found");
        }

        [Fact]
        public async Task UpdateAsync_ExistingPatient_ReturnsSuccessWithUpdatedPatient()
        {
            var patientId = 1;
            var existingPatient = CreateTestPatient(patientId, "Old", "Name", 1, "Homme");

            var updateDto = new CreatePatientDto
            {
                FirstName = "Updated",
                LastName = "Name",
                BirthDate = new DateTime(1990, 1, 1),
                GenderId = 1,
                Address = "New Address",
                Phone = "555-9999"
            };

            _mockRepository
                .Setup(repo => repo.GetByIdAsync(patientId))
                .ReturnsAsync(Result<Patient>.Success(existingPatient));

            _mockRepository
                .Setup(repo => repo.GenderExistsAsync(1))
                .ReturnsAsync(Result<bool>.Success(true));

            _mockRepository
                .Setup(repo => repo.UpdateAsync(It.IsAny<Patient>()))
                .ReturnsAsync((Patient p) => Result<Patient>.Success(p));

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
                GenderId = 1
            };

            _mockRepository
                .Setup(repo => repo.GetByIdAsync(patientId))
                .ReturnsAsync(Result<Patient>.Failure("Patient not found"));

            var result = await _service.UpdateAsync(patientId, updateDto);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Patient not found");
        }

        [Fact]
        public async Task UpdateAsync_InvalidGenderId_ReturnsFailure()
        {
            var patientId = 1;
            var existingPatient = CreateTestPatient(patientId, "Old", "Name", 1, "Homme");

            var updateDto = new CreatePatientDto
            {
                FirstName = "Updated",
                LastName = "Name",
                BirthDate = new DateTime(1990, 1, 1),
                GenderId = 999
            };

            _mockRepository
                .Setup(repo => repo.GetByIdAsync(patientId))
                .ReturnsAsync(Result<Patient>.Success(existingPatient));

            _mockRepository
                .Setup(repo => repo.GenderExistsAsync(999))
                .ReturnsAsync(Result<bool>.Failure("Gender not found"));

            var result = await _service.UpdateAsync(patientId, updateDto);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Gender not found");
        }

        [Fact]
        public async Task DeleteAsync_ExistingPatient_ReturnsSuccess()
        {
            var patientId = 1;
            _mockRepository
                .Setup(repo => repo.DeleteAsync(patientId))
                .ReturnsAsync(Result<bool>.Success(true));

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
                .ReturnsAsync(Result<bool>.Failure("Patient not found"));

            var result = await _service.DeleteAsync(patientId);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Patient not found");
        }

        [Fact]
        public async Task GetAllGendersAsync_ReturnsSuccessWithListOfGenders()
        {
            var genders = new List<Gender>
            {
                new Gender { Id = 1, Name = "Homme" },
                new Gender { Id = 2, Name = "Femme" },
                new Gender { Id = 3, Name = "Autre" }
            };

            _mockRepository
                .Setup(repo => repo.GetAllGendersAsync())
                .ReturnsAsync(Result<IEnumerable<Gender>>.Success(genders));

            var result = await _service.GetAllGendersAsync();

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().HaveCount(3);
            result.Value.Should().Contain(g => g.Name == "Homme");
        }
    }
}
