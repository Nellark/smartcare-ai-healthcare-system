using Microsoft.EntityFrameworkCore;
using SmartCare.Domain.ValueObjects;
using SmartCare.Infrastructure.Persistence.Entities;

namespace SmartCare.Infrastructure.Persistence;

public static class DataSeed
{
    public static async Task SeedAsync(SmartCareDbContext context)
    {
        await SeedPatientsAsync(context);
        await SeedDoctorsAsync(context);
    }

    private static async Task SeedPatientsAsync(SmartCareDbContext context)
    {
        if (await context.Patients.AnyAsync())
        {
            return; // Database already seeded
        }

        var patients = new List<PatientEntity>
        {
            new()
            {
                Id = Domain.ValueObjects.PatientId.FromGuid(Guid.Parse("11111111-1111-1111-1111-111111111111")),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                DateOfBirth = new DateTime(1980, 1, 15),
                PhoneNumber = "555-0101",
                Address = "123 Main St, Anytown, USA",
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                MedicalRecords = new List<MedicalRecordEntity>
                {
                    new()
                    {
                        Id = Domain.ValueObjects.MedicalRecordId.FromGuid(Guid.Parse("11111111-1111-1111-1111-111111111112")),
                        Diagnosis = "Hypertension",
                        Treatment = "Prescribed ACE inhibitors",
                        Notes = "Patient shows elevated blood pressure",
                        RecordDate = DateTime.UtcNow.AddDays(-25),
                        DoctorId = "doctor-001",
                        CreatedAt = DateTime.UtcNow.AddDays(-25)
                    }
                }
            },
            new()
            {
                Id = Domain.ValueObjects.PatientId.FromGuid(Guid.Parse("22222222-2222-2222-2222-222222222222")),
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                DateOfBirth = new DateTime(1992, 5, 22),
                PhoneNumber = "555-0102",
                Address = "456 Oak Ave, Somewhere, USA",
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                MedicalRecords = new List<MedicalRecordEntity>
                {
                    new()
                    {
                        Id = Domain.ValueObjects.MedicalRecordId.FromGuid(Guid.Parse("22222222-2222-2222-2222-222222222223")),
                        Diagnosis = "Type 2 Diabetes",
                        Treatment = "Metformin prescription",
                        Notes = "Patient managing blood sugar levels",
                        RecordDate = DateTime.UtcNow.AddDays(-15),
                        DoctorId = "doctor-002",
                        CreatedAt = DateTime.UtcNow.AddDays(-15)
                    }
                }
            },
            new()
            {
                Id = Domain.ValueObjects.PatientId.FromGuid(Guid.Parse("33333333-3333-3333-3333-333333333333")),
                FirstName = "Robert",
                LastName = "Johnson",
                Email = "robert.johnson@example.com",
                DateOfBirth = new DateTime(1975, 8, 10),
                PhoneNumber = "555-0103",
                Address = "789 Pine Rd, Elsewhere, USA",
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            }
        };

        await context.Patients.AddRangeAsync(patients);
        await context.SaveChangesAsync();
    }

    private static async Task SeedDoctorsAsync(SmartCareDbContext context)
    {
        if (await context.Doctors.AnyAsync())
        {
            return;
        }

        var doctors = new List<DoctorEntity>
        {
            new()
            {
                Id = Domain.ValueObjects.DoctorId.FromGuid(Guid.Parse("44444444-4444-4444-4444-444444444441")),
                FirstName = "Ava",
                LastName = "Mokoena",
                Email = "ava.mokoena@smartcare.com",
                Specialty = "General Practice",
                PhoneNumber = "555-0201",
                LicenseNumber = "LIC-GP-001",
                CreatedAt = DateTime.UtcNow.AddDays(-40)
            },
            new()
            {
                Id = Domain.ValueObjects.DoctorId.FromGuid(Guid.Parse("44444444-4444-4444-4444-444444444442")),
                FirstName = "Thabo",
                LastName = "Ndlovu",
                Email = "thabo.ndlovu@smartcare.com",
                Specialty = "Cardiology",
                PhoneNumber = "555-0202",
                LicenseNumber = "LIC-CARD-002",
                CreatedAt = DateTime.UtcNow.AddDays(-35)
            },
            new()
            {
                Id = Domain.ValueObjects.DoctorId.FromGuid(Guid.Parse("44444444-4444-4444-4444-444444444443")),
                FirstName = "Lerato",
                LastName = "Naidoo",
                Email = "lerato.naidoo@smartcare.com",
                Specialty = "Pediatrics",
                PhoneNumber = "555-0203",
                LicenseNumber = "LIC-PED-003",
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            }
        };

        await context.Doctors.AddRangeAsync(doctors);
        await context.SaveChangesAsync();
    }
}
