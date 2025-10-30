using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MediLabo.Patients.API.Data;
using MediLabo.Patients.API.Models.Entities;
using MediLabo.Patients.API.Repositories;
using Xunit;

namespace MediLabo.Patients.Tests.Repositories
{
    [Trait("Category", "Unit")]
    public class PatientRepositoryTests : IDisposable
    {
        private readonly PatientDbContext _context;
        private readonly PatientRepository _repository;

        public PatientRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<PatientDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new PatientDbContext(options);
            _repository = new PatientRepository(_context);

            SeedTestData();
        }

        private void SeedTestData()
        {
            var genders = new[]
            {
                new Gender { Id = 1, Name = "Homme" },
                new Gender { Id = 2, Name = "Femme" },
                new Gender { Id = 3, Name = "Autre" }
            };

            _context.Genders.AddRange(genders);
            _context.SaveChanges();

            var patients = new[]
            {
                new Patient
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    BirthDate = new DateTime(1990, 1, 1),
                    GenderId = 1,
                    Address = "123 Main St",
                    Phone = "555-1234"
                },
                new Patient
                {
                    Id = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    BirthDate = new DateTime(1985, 5, 15),
                    GenderId = 2,
                    Address = "456 Oak Ave",
                    Phone = "555-5678"
                }
            };

            _context.Patients.AddRange(patients);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllAsync_ExcludesSoftDeletedPatients()
        {
            var patientToDelete = await _context.Patients.FindAsync(1);
            patientToDelete!.IsDeleted = true;
            patientToDelete.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync();

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);
            result.Value.Should().NotContain(p => p.Id == 1);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsSuccessWithPatient()
        {
            var patientId = 1;

            var result = await _repository.GetByIdAsync(patientId);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Id.Should().Be(patientId);
            result.Value.FirstName.Should().Be("John");
            result.Value.LastName.Should().Be("Doe");
            result.Value.Gender.Should().NotBeNull();
            result.Value.Gender.Name.Should().Be("Homme");
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingId_ReturnsFailure()
        {
            var nonExistingId = 999;

            var result = await _repository.GetByIdAsync(nonExistingId);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Patient not found");
        }

        [Fact]
        public async Task CreateAsync_ValidPatient_ReturnsSuccessWithCreatedPatient()
        {
            var newPatient = new Patient
            {
                FirstName = "New",
                LastName = "Patient",
                BirthDate = new DateTime(1995, 3, 20),
                GenderId = 1,
                Address = "789 Elm St",
                Phone = "555-9999"
            };

            var result = await _repository.CreateAsync(newPatient);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Id.Should().BeGreaterThan(0);
            result.Value.FirstName.Should().Be("New");
            result.Value.Gender.Should().NotBeNull();
            result.Value.Gender!.Name.Should().Be("Homme");

            var savedPatient = await _context.Patients.Include(p => p.Gender).FirstOrDefaultAsync(p => p.Id == result.Value.Id);
            savedPatient.Should().NotBeNull();
            savedPatient!.FirstName.Should().Be("New");
        }

        [Fact]
        public async Task UpdateAsync_ExistingPatient_ReturnsSuccessWithUpdatedPatient()
        {
            var patientToUpdate = await _context.Patients.Include(p => p.Gender).FirstOrDefaultAsync(p => p.Id == 1);
            patientToUpdate!.FirstName = "Updated";
            patientToUpdate.Address = "New Address";

            var result = await _repository.UpdateAsync(patientToUpdate);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.FirstName.Should().Be("Updated");
            result.Value.Address.Should().Be("New Address");
            result.Value.Gender.Should().NotBeNull();

            var updatedPatient = await _context.Patients.Include(p => p.Gender).FirstOrDefaultAsync(p => p.Id == 1);
            updatedPatient!.FirstName.Should().Be("Updated");
            updatedPatient.Address.Should().Be("New Address");
        }

        [Fact]
        public async Task DeleteAsync_ExistingPatient_ReturnsSuccess()
        {
            var patientId = 1;

            var result = await _repository.DeleteAsync(patientId);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();

            var deletedPatient = await _context.Patients.FindAsync(patientId);
            deletedPatient.Should().NotBeNull();
            deletedPatient!.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_NonExistingPatient_ReturnsFailure()
        {
            var nonExistingId = 999;

            var result = await _repository.DeleteAsync(nonExistingId);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Patient not found");
        }

        [Fact]
        public async Task GetAllGendersAsync_ReturnsSuccessWithAllGenders()
        {
            var result = await _repository.GetAllGendersAsync();

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().HaveCount(3);
            result.Value.Should().Contain(g => g.Name == "Homme");
            result.Value.Should().Contain(g => g.Name == "Femme");
            result.Value.Should().Contain(g => g.Name == "Autre");
        }

        [Fact]
        public async Task GenderExistsAsync_ExistingGender_ReturnsSuccess()
        {
            var result = await _repository.GenderExistsAsync(1);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();
        }

        [Fact]
        public async Task GenderExistsAsync_NonExistingGender_ReturnsFailure()
        {
            var result = await _repository.GenderExistsAsync(999);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Gender not found");
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
