using FluentAssertions;
using MediLabo.Notes.API.Data;
using MediLabo.Notes.API.Models.Entities;
using MediLabo.Notes.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MediLabo.Notes.Tests.Repositories
{
    [Trait("Category", "Unit")]
    public class NoteRepositoryTests : IDisposable
    {
        private readonly NotesDbContext _context;
        private readonly NoteRepository _repository;

        public NoteRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<NotesDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new NotesDbContext(options);
            _repository = new NoteRepository(_context);

            SeedTestData();
        }

        private void SeedTestData()
        {
            var notes = new List<Note>
            {
                new Note { Id = "n1", PatientId = 1, Content = "First", CreatedAt = DateTime.UtcNow.AddMinutes(-10) },
                new Note { Id = "n2", PatientId = 1, Content = "Second", CreatedAt = DateTime.UtcNow },
                new Note { Id = "n3", PatientId = 2, Content = "Other patient", CreatedAt = DateTime.UtcNow.AddMinutes(-5) },
                new Note { Id = "n4", PatientId = 1, Content = "Deleted", IsDeleted = true, DeletedAt = DateTime.UtcNow }
            };

            _context.Notes.AddRange(notes);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetNotesByPatientIdAsync_ReturnsOrderedAndExcludesDeleted()
        {
            var result = await _repository.GetNotesByPatientIdAsync(1);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            var list = result.Value!.ToList();
            list.Should().HaveCount(2);
            list[0].Id.Should().Be("n2");
            list[1].Id.Should().Be("n1");
        }

        [Fact]
        public async Task GetNoteByIdAsync_Existing_ReturnsSuccess()
        {
            var result = await _repository.GetNoteByIdAsync("n1");

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Id.Should().Be("n1");
        }

        [Fact]
        public async Task GetNoteByIdAsync_NotFound_ReturnsFailure()
        {
            var result = await _repository.GetNoteByIdAsync("missing");

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Note not found");
        }

        [Fact]
        public async Task CreateNoteAsync_Valid_ReturnsCreatedNote()
        {
            var newNote = new Note { Id = "n5", PatientId = 1, Content = "New" };

            var result = await _repository.CreateNoteAsync(newNote);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Id.Should().Be("n5");

            var saved = await _context.Notes.FirstOrDefaultAsync(n => n.Id == "n5");
            saved.Should().NotBeNull();
            saved!.Content.Should().Be("New");
        }

        [Fact]
        public async Task UpdateNoteAsync_Existing_UpdatesContentAndTimestamp()
        {
            var updated = new Note { PatientId = 1, Content = "Updated" };

            var result = await _repository.UpdateNoteAsync("n1", updated);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Content.Should().Be("Updated");
            result.Value.UpdatedAt.Should().NotBeNull();

            var saved = await _context.Notes.FirstAsync(n => n.Id == "n1");
            saved.Content.Should().Be("Updated");
            saved.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateNoteAsync_NotFound_ReturnsFailure()
        {
            var updated = new Note { PatientId = 1, Content = "Updated" };

            var result = await _repository.UpdateNoteAsync("missing", updated);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Note not found");
        }

        [Fact]
        public async Task DeleteNoteAsync_Existing_SoftDeletes()
        {
            var result = await _repository.DeleteNoteAsync("n2");

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();

            var deleted = await _context.Notes.IgnoreQueryFilters().FirstAsync(n => n.Id == "n2");
            deleted.IsDeleted.Should().BeTrue();
            deleted.DeletedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteNoteAsync_NotFound_ReturnsFailure()
        {
            var result = await _repository.DeleteNoteAsync("missing");

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Note not found");
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
