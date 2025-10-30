using MediLabo.Notes.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using SharpCompress.Common;

namespace MediLabo.Notes.API.Data;

public class NotesDbContext : DbContext
{
    public NotesDbContext(DbContextOptions<NotesDbContext> options) : base(options)
    {
    }

    public DbSet<Note> Notes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Note>(entity =>
        {
            entity.HasQueryFilter(n => !n.IsDeleted);

            entity.Property(n => n.Id)
                .ValueGeneratedOnAdd();
        });
    }
}
