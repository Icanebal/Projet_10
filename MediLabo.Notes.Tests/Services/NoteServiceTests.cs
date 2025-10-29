using FluentAssertions;
using MediLabo.Common;
using MediLabo.Common.HttpServices;
using MediLabo.Notes.API.Interfaces;
using MediLabo.Notes.API.Models.DTOs;
using MediLabo.Notes.API.Models.Entities;
using MediLabo.Notes.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace MediLabo.Notes.Tests.Services
{
    [Trait("Category", "Unit")]
    public class NoteServiceTests
    {
        private readonly Mock<INoteRepository> _mockRepository;
        private readonly Mock<ILogger<NoteService>> _mockLogger;
        private readonly Mock<IPatientApiService> _mockPatientApiService;
        private readonly NoteService _service;
        

        public NoteServiceTests()
        {
            _mockRepository = new Mock<INoteRepository>();
            _mockLogger = new Mock<ILogger<NoteService>>();
            _mockPatientApiService = new Mock<IPatientApiService>();
            _service = new NoteService(_mockRepository.Object, _mockPatientApiService.Object, _mockLogger.Object);
        }

        private static Note CreateTestNote(string id, int patientId = 1, string content = "Note")
        {
            return new Note
            {
                Id = id,
                PatientId = patientId,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };
        }

        [Fact]
        public async Task GetNotesByPatientIdAsync_ReturnsSuccessWithDtos()
        {
            _mockPatientApiService
               .Setup(p => p.GetPatientFullNameAsync(1))
               .ReturnsAsync(Result<string>.Success("John Doe"));
            _mockRepository
                .Setup(r => r.GetNotesByPatientIdAsync(1))
                .ReturnsAsync(Result<IEnumerable<Note>>.Success(new[]
                {
                    CreateTestNote("n1"),
                    CreateTestNote("n2")
                }));

            var result = await _service.GetNotesByPatientIdAsync(1);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Count().Should().Be(2);
        }

        [Fact]
        public async Task GetNotesByPatientIdAsync_Failure_BubblesError()
        {
            _mockPatientApiService
               .Setup(p => p.GetPatientFullNameAsync(1))
               .ReturnsAsync(Result<string>.Success("John Doe"));
            _mockRepository
                .Setup(r => r.GetNotesByPatientIdAsync(1))
                .ReturnsAsync(Result<IEnumerable<Note>>.Failure("DB error"));

            var result = await _service.GetNotesByPatientIdAsync(1);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("DB error");
        }

        [Fact]
        public async Task GetNoteByIdAsync_Existing_ReturnsDto()
        {
            _mockPatientApiService
               .Setup(p => p.GetPatientFullNameAsync(1))
               .ReturnsAsync(Result<string>.Success("John Doe"));
            _mockRepository
                .Setup(r => r.GetNoteByIdAsync("n1"))
                .ReturnsAsync(Result<Note>.Success(CreateTestNote("n1")));

            var result = await _service.GetNoteByIdAsync("n1");

            result.IsSuccess.Should().BeTrue();
            result.Value!.Id.Should().Be("n1");
        }

        [Fact]
        public async Task GetNoteByIdAsync_NotFound_ReturnsFailure()
        {
            _mockRepository
                .Setup(r => r.GetNoteByIdAsync("missing"))
                .ReturnsAsync(Result<Note>.Failure("Note not found"));

            var result = await _service.GetNoteByIdAsync("missing");

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Note not found");
        }

        [Fact]
        public async Task CreateNoteAsync_Valid_ReturnsCreatedDto()
        {
            var dto = new CreateNoteDto { PatientId = 1, Content = "New" };
            var created = CreateTestNote("new-id", 1, "New");

            _mockPatientApiService
               .Setup(p => p.GetPatientFullNameAsync(1))
               .ReturnsAsync(Result<string>.Success("John Doe"));
            _mockRepository
                .Setup(r => r.CreateNoteAsync(It.IsAny<Note>()))
                .ReturnsAsync(Result<Note>.Success(created));

            var result = await _service.CreateNoteAsync(dto);

            result.IsSuccess.Should().BeTrue();
            result.Value!.Id.Should().Be("new-id");
        }

        [Fact]
        public async Task CreateNoteAsync_Failure_BubblesError()
        {
            var dto = new CreateNoteDto { PatientId = 1, Content = "New" };

            _mockPatientApiService
               .Setup(p => p.GetPatientFullNameAsync(1))
               .ReturnsAsync(Result<string>.Success("John Doe"));
            _mockRepository
                .Setup(r => r.CreateNoteAsync(It.IsAny<Note>()))
                .ReturnsAsync(Result<Note>.Failure("Insert error"));

            var result = await _service.CreateNoteAsync(dto);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Insert error");
        }

        [Fact]
        public async Task UpdateNoteAsync_Valid_ReturnsUpdatedDto()
        {
            var dto = new CreateNoteDto { PatientId = 1, Content = "Updated" };
            var updated = CreateTestNote("n1", 1, "Updated");

            _mockPatientApiService
               .Setup(p => p.GetPatientFullNameAsync(1))
               .ReturnsAsync(Result<string>.Success("John Doe"));
            _mockRepository
                .Setup(r => r.UpdateNoteAsync("n1", It.IsAny<Note>()))
                .ReturnsAsync(Result<Note>.Success(updated));

            var result = await _service.UpdateNoteAsync("n1", dto);

            result.IsSuccess.Should().BeTrue();
            result.Value!.Content.Should().Be("Updated");
        }

        [Fact]
        public async Task UpdateNoteAsync_NotFound_ReturnsFailure()
        {
            var dto = new CreateNoteDto { PatientId = 1, Content = "Updated" };

            _mockPatientApiService
               .Setup(p => p.GetPatientFullNameAsync(1))
               .ReturnsAsync(Result<string>.Success("John Doe"));
            _mockRepository
                .Setup(r => r.UpdateNoteAsync("missing", It.IsAny<Note>()))
                .ReturnsAsync(Result<Note>.Failure("Note not found"));

            var result = await _service.UpdateNoteAsync("missing", dto);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Note not found");
        }

        [Fact]
        public async Task DeleteNoteAsync_Existing_ReturnsSuccess()
        {
            _mockRepository
                .Setup(r => r.DeleteNoteAsync("n1"))
                .ReturnsAsync(Result<bool>.Success(true));

            var result = await _service.DeleteNoteAsync("n1");

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteNoteAsync_NotFound_ReturnsFailure()
        {
            _mockRepository
                .Setup(r => r.DeleteNoteAsync("missing"))
                .ReturnsAsync(Result<bool>.Failure("Note not found"));

            var result = await _service.DeleteNoteAsync("missing");

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Note not found");
        }
    }
}
