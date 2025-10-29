using FluentAssertions;
using MediLabo.Common;
using MediLabo.Common.DTOs;
using MediLabo.Common.HttpServices;
using MediLabo.Notes.API.Controllers;
using MediLabo.Notes.API.Interfaces;
using MediLabo.Notes.API.Models.DTOs;
using MediLabo.Notes.API.Models.Entities;
using MediLabo.Notes.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace MediLabo.Notes.Tests.Controllers
{
    [Trait("Category", "Unit")]
    public class NotesControllerTests
    {
        private readonly Mock<INoteRepository> _mockRepository;
        private readonly Mock<ILogger<NoteService>> _mockLogger;
        private readonly Mock<IPatientApiService> _mockPatientApiService;
        private readonly NoteService _service;
        private readonly NotesController _controller;

        public NotesControllerTests()
        {
            _mockRepository = new Mock<INoteRepository>();
            _mockLogger = new Mock<ILogger<NoteService>>();
            _mockPatientApiService = new Mock<IPatientApiService>();
            _service = new NoteService(_mockRepository.Object, _mockPatientApiService.Object, _mockLogger.Object);
            _controller = new NotesController(_service);
        }

        private static Note CreateTestNote(string id, int patientId, string content, DateTime? createdAt = null)
        {
            return new Note
            {
                Id = id,
                PatientId = patientId,
                Content = content,
                CreatedAt = createdAt ?? DateTime.UtcNow
            };
        }

        [Fact]
        public async Task GetNotesByPatientId_ReturnsOkWithNotes()
        {
            var notes = new List<Note>
            {
                CreateTestNote("n1", 1, "Note A")
            };

            _mockPatientApiService
               .Setup(p => p.GetPatientFullNameAsync(1))
               .ReturnsAsync(Result<string>.Success("John Doe"));
            _mockRepository
                .Setup(r => r.GetNotesByPatientIdAsync(1))
                .ReturnsAsync(Result<IEnumerable<Note>>.Success(notes));

            var actionResult = await _controller.GetNotesByPatientId(1);

            var ok = actionResult.Result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.StatusCode.Should().Be(200);

            var value = ok.Value as Result<IEnumerable<NoteDto>>;
            value.Should().NotBeNull();
            value!.IsSuccess.Should().BeTrue();
            value.Value.Should().HaveCount(1);
            value.Value!.First().PatientId.Should().Be(1);
        }

        [Fact]
        public async Task GetNoteById_Existing_ReturnsOk()
        {
            var note = CreateTestNote("n1", 1, "Content");

            _mockPatientApiService
               .Setup(p => p.GetPatientFullNameAsync(1))
               .ReturnsAsync(Result<string>.Success("John Doe"));
            _mockRepository
                .Setup(r => r.GetNoteByIdAsync("n1"))
                .ReturnsAsync(Result<Note>.Success(note));

            var actionResult = await _controller.GetNoteById("n1");

            var ok = actionResult.Result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.StatusCode.Should().Be(200);

            var value = ok.Value as Result<NoteDto>;
            value.Should().NotBeNull();
            value!.IsSuccess.Should().BeTrue();
            value.Value!.Id.Should().Be("n1");
        }

        [Fact]
        public async Task GetNoteById_NotFound_ReturnsNotFound()
        {
            _mockRepository
                .Setup(r => r.GetNoteByIdAsync("missing"))
                .ReturnsAsync(Result<Note>.Failure("Note not found"));

            var actionResult = await _controller.GetNoteById("missing");

            var notFound = actionResult.Result as NotFoundObjectResult;
            notFound.Should().NotBeNull();
            notFound!.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task CreateNote_Valid_ReturnsCreatedAt()
        {
            var dto = new CreateNoteDto
            {
                PatientId = 1,
                Content = "New note"
            };

            var created = CreateTestNote("new-id", 1, "New note");

            _mockPatientApiService
                .Setup(p => p.GetPatientFullNameAsync(1))
                .ReturnsAsync(Result<string>.Success("John Doe"));

            _mockRepository
                .Setup(r => r.CreateNoteAsync(It.IsAny<Note>()))
                .ReturnsAsync(Result<Note>.Success(created));

            var actionResult = await _controller.CreateNote(dto);

            var createdAt = actionResult.Result as CreatedAtActionResult;
            createdAt.Should().NotBeNull();
            createdAt!.StatusCode.Should().Be(201);
            createdAt.ActionName.Should().Be(nameof(NotesController.GetNoteById));

            var value = createdAt.Value as Result<NoteDto>;
            value.Should().NotBeNull();
            value!.IsSuccess.Should().BeTrue();
            value.Value!.Id.Should().Be("new-id");
        }

        [Fact]
        public async Task CreateNote_InvalidModelState_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Content", "Required");

            var dto = new CreateNoteDto
            {
                PatientId = 1,
                Content = ""
            };

            var actionResult = await _controller.CreateNote(dto);

            var bad = actionResult.Result as BadRequestObjectResult;
            bad.Should().NotBeNull();
            bad!.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task UpdateNote_Existing_ReturnsOk()
        {
            var dto = new CreateNoteDto
            {
                PatientId = 1,
                Content = "Updated"
            };

            var updated = CreateTestNote("n1", 1, "Updated");

            _mockPatientApiService
               .Setup(p => p.GetPatientFullNameAsync(1))
               .ReturnsAsync(Result<string>.Success("John Doe"));
            _mockRepository
                .Setup(r => r.UpdateNoteAsync("n1", It.IsAny<Note>()))
                .ReturnsAsync(Result<Note>.Success(updated));

            var actionResult = await _controller.UpdateNote("n1", dto);

            var ok = actionResult.Result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.StatusCode.Should().Be(200);

            var value = ok.Value as Result<NoteDto>;
            value.Should().NotBeNull();
            value!.Value!.Content.Should().Be("Updated");
        }

        [Fact]
        public async Task UpdateNote_InvalidModelState_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Content", "Required");

            var dto = new CreateNoteDto
            {
                PatientId = 1,
                Content = ""
            };

            var actionResult = await _controller.UpdateNote("n1", dto);

            var bad = actionResult.Result as BadRequestObjectResult;
            bad.Should().NotBeNull();
            bad!.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task UpdateNote_NotFound_ReturnsNotFound()
        {
            var dto = new CreateNoteDto
            {
                PatientId = 1,
                Content = "Updated"
            };

            _mockPatientApiService
               .Setup(p => p.GetPatientFullNameAsync(1))
               .ReturnsAsync(Result<string>.Success("John Doe"));
            _mockRepository
                .Setup(r => r.UpdateNoteAsync("missing", It.IsAny<Note>()))
                .ReturnsAsync(Result<Note>.Failure("Note not found"));

            var action = await _controller.UpdateNote("missing", dto);

            var notFound = action.Result as NotFoundObjectResult;
            notFound.Should().NotBeNull();
            notFound!.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task DeleteNote_Existing_ReturnsOkWithTrue()
        {
            _mockRepository
                .Setup(r => r.DeleteNoteAsync("n1"))
                .ReturnsAsync(Result<bool>.Success(true));

            var action = await _controller.DeleteNote("n1");

            var ok = action.Result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.StatusCode.Should().Be(200);

            var value = ok.Value as Result<bool>;
            value.Should().NotBeNull();
            value!.IsSuccess.Should().BeTrue();
            value.Value.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteNote_NotFound_ReturnsNotFound()
        {
            _mockRepository
                .Setup(r => r.DeleteNoteAsync("missing"))
                .ReturnsAsync(Result<bool>.Failure("Note not found"));

            var action = await _controller.DeleteNote("missing");

            var notFound = action.Result as NotFoundObjectResult;
            notFound.Should().NotBeNull();
            notFound!.StatusCode.Should().Be(404);
        }
    }
}
