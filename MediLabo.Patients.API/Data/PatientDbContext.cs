using Microsoft.EntityFrameworkCore;
using MediLabo.Patients.API.Models.Entities;

namespace MediLabo.Patients.API.Data
{
    public class PatientDbContext : DbContext
    {
        public PatientDbContext(DbContextOptions<PatientDbContext> options) : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }

        public DbSet<Gender> Genders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.FirstName)
                    .IsRequired();

                entity.Property(e => e.LastName)
                    .IsRequired();

                entity.Property(e => e.BirthDate)
                    .IsRequired();

                entity.HasOne(p => p.Gender)
                    .WithMany(g => g.Patients)
                    .HasForeignKey(p => p.GenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(p => !p.IsDeleted);
            });

            modelBuilder.Entity<Gender>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasIndex(e => e.Name)
                    .IsUnique();
            });

            SeedGenders(modelBuilder);
            SeedTestData(modelBuilder);
        }

        private static void SeedGenders(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Gender>().HasData(
                new Gender { Id = 1, Name = "Homme" },
                new Gender { Id = 2, Name = "Femme" },
                new Gender { Id = 3, Name = "Autre" }
            );
        }

        private static void SeedTestData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>().HasData(
                new Patient
                {
                    Id = 1,
                    FirstName = "Test",
                    LastName = "TestNone",
                    BirthDate = new DateTime(1966, 12, 31),
                    GenderId = 2,
                    Address = "1 Brookside St",
                    Phone = "100-222-3333",
                },
                new Patient
                {
                    Id = 2,
                    FirstName = "Test",
                    LastName = "TestBorderline",
                    BirthDate = new DateTime(1945, 6, 24),
                    GenderId = 1,
                    Address = "2 High St",
                    Phone = "200-333-4444",
                },
                new Patient
                {
                    Id = 3,
                    FirstName = "Test",
                    LastName = "TestInDanger",
                    BirthDate = new DateTime(2004, 6, 18),
                    GenderId = 1,
                    Address = "3 Club Road",
                    Phone = "300-444-5555",
                },
                new Patient
                {
                    Id = 4,
                    FirstName = "Test",
                    LastName = "TestEarlyOnset",
                    BirthDate = new DateTime(2002, 6, 28),
                    GenderId = 2,
                    Address = "4 Valley Dr",
                    Phone = "400-555-6666",
                }
            );
        }
    }
}
