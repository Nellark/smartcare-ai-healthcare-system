using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SmartCare.Domain.Entities;
using SmartCare.Domain.ValueObjects;
using SmartCare.Infrastructure.Persistence;
using SmartCare.Infrastructure.Persistence.Entities;
using SmartCare.Infrastructure.Persistence.Repositories;
using Xunit;

namespace SmartCare.Infrastructure.Tests;

public sealed class PatientRepositoryRehydrationTests
{
    [Fact]
    public async Task GetByIdAsync_RehydratesPatientWithoutRaisingDomainEvents()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<SmartCareDbContext>()
            .UseSqlite(connection)
            .Options;

        await using (var context = new SmartCareDbContext(options))
        {
            await context.Database.EnsureCreatedAsync();

            var patientId = PatientId.FromGuid(Guid.Parse("11111111-1111-1111-1111-111111111111"));
            var recordId = MedicalRecordId.FromGuid(Guid.Parse("22222222-2222-2222-2222-222222222222"));

            context.Patients.Add(new PatientEntity
            {
                Id = patientId,
                FirstName = "JanYonelae",
                LastName = "Kulati",
                Email = "nelakulati@gmail.com",
                DateOfBirth = new DateTime(1990, 4, 12),
                PhoneNumber = "555-0100",
                Address = "14 2nd Street",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                MedicalRecords = new List<MedicalRecordEntity>
                {
                    new()
                    {
                        Id = recordId,
                        PatientId = patientId,
                        Diagnosis = "Hypertension",
                        Treatment = "Medication",
                        Notes = "Initial diagnosis",
                        RecordDate = new DateTime(2024, 1, 3, 0, 0, 0, DateTimeKind.Utc),
                        DoctorId = "doctor-1",
                        CreatedAt = new DateTime(2024, 1, 3, 0, 0, 0, DateTimeKind.Utc),
                        UpdatedAt = new DateTime(2024, 1, 4, 0, 0, 0, DateTimeKind.Utc)
                    }
                }
            });

            await context.SaveChangesAsync();
        }

        await using (var context = new SmartCareDbContext(options))
        {
            var repository = new PatientRepository(context);

            var patient = await repository.GetByIdAsync(PatientId.FromGuid(Guid.Parse("11111111-1111-1111-1111-111111111111")));

            Assert.NotNull(patient);
            Assert.Equal("Yonela", patient!.Name.FirstName);
            Assert.Equal("Kulati", patient.Name.LastName);
            Assert.Equal("nelakulati@gmail.com", patient.Email.Value);
            Assert.Equal(1, patient.MedicalRecords.Count);
            Assert.Empty(patient.GetDomainEvents());

            var medicalRecord = Assert.Single(patient.MedicalRecords);
            Assert.Equal("Hypertension", medicalRecord.Diagnosis);
            Assert.Equal("doctor-1", medicalRecord.DoctorId);
        }
    }
}
