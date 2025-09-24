using Microsoft.EntityFrameworkCore;
using Patients.API.Models.Entities;

namespace Patients.API.Data
{
    public class PatientDbContext : DbContext
    {
        public PatientDbContext(DbContextOptions<PatientDbContext> options) : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }

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

                entity.Property(e => e.Gender)
                    .IsRequired();
            });

            SeedTestData(modelBuilder);
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
                    Gender = "F",
                    Address = "1 Brookside St",
                    Phone = "100-222-3333",
                },
                new Patient
                {
                    Id = 2,
                    FirstName = "Test",
                    LastName = "TestBorderline",
                    BirthDate = new DateTime(1945, 6, 24),
                    Gender = "M",
                    Address = "2 High St",
                    Phone = "200-333-4444",
                },
                new Patient
                {
                    Id = 3,
                    FirstName = "Test",
                    LastName = "TestInDanger",
                    BirthDate = new DateTime(2004, 6, 18),
                    Gender = "M",
                    Address = "3 Club Road",
                    Phone = "300-444-5555",
                },
                new Patient
                {
                    Id = 4,
                    FirstName = "Test",
                    LastName = "TestEarlyOnset",
                    BirthDate = new DateTime(2002, 6, 28),
                    Gender = "F",
                    Address = "4 Valley Dr",
                    Phone = "400-555-6666",
                }
            );
        }
    }
}
