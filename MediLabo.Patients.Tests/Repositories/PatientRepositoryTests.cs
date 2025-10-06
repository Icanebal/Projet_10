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
            var patients = new[]
            {
                new Patient
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    BirthDate = new DateTime(1990, 1, 1),
                    Gender = "M",
                    Address = "123 Main St",
                    Phone = "555-1234"
                },
                new Patient
                {
                    Id = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    BirthDate = new DateTime(1985, 5, 15),
                    Gender = "F",
                    Address = "456 Oak Ave",
                    Phone = "555-5678"
                }
            };

            _context.Patients.AddRange(patients);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllPatients()
        {
            var result = await _repository.GetAllAsync();

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsPatient()
        {
            var patientId = 1;

            var result = await _repository.GetByIdAsync(patientId);

            result.Should().NotBeNull();
            result!.Id.Should().Be(patientId);
            result.FirstName.Should().Be("John");
            result.LastName.Should().Be("Doe");
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            var nonExistingId = 999;

            var result = await _repository.GetByIdAsync(nonExistingId);

            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_ValidPatient_ReturnsCreatedPatient()
        {
            var newPatient = new Patient
            {
                FirstName = "New",
                LastName = "Patient",
                BirthDate = new DateTime(1995, 3, 20),
                Gender = "M",
                Address = "789 Elm St",
                Phone = "555-9999"
            };

            var result = await _repository.CreateAsync(newPatient);

            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.FirstName.Should().Be("New");

            var savedPatient = await _context.Patients.FindAsync(result.Id);
            savedPatient.Should().NotBeNull();
            savedPatient!.FirstName.Should().Be("New");
        }

        [Fact]
        public async Task UpdateAsync_ExistingPatient_ReturnsUpdatedPatient()
        {
            var patientToUpdate = await _context.Patients.FindAsync(1);
            patientToUpdate!.FirstName = "Updated";
            patientToUpdate.Address = "New Address";

            var result = await _repository.UpdateAsync(patientToUpdate);

            result.Should().NotBeNull();
            result.FirstName.Should().Be("Updated");
            result.Address.Should().Be("New Address");

            var updatedPatient = await _context.Patients.FindAsync(1);
            updatedPatient!.FirstName.Should().Be("Updated");
            updatedPatient.Address.Should().Be("New Address");
        }

        [Fact]
        public async Task DeleteAsync_ExistingPatient_ReturnsTrue()
        {
            var patientId = 1;

            var result = await _repository.DeleteAsync(patientId);

            result.Should().BeTrue();

            var deletedPatient = await _context.Patients.FindAsync(patientId);
            deletedPatient.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_NonExistingPatient_ReturnsFalse()
        {
            var nonExistingId = 999;

            var result = await _repository.DeleteAsync(nonExistingId);

            result.Should().BeFalse();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}