using SmartCare.Domain.ValueObjects;

namespace SmartCare.Infrastructure.Persistence.Entities;

public class PatientEntity : IAuditableEntity
{
    public PatientId Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    public ICollection<MedicalRecordEntity> MedicalRecords { get; set; } = new List<MedicalRecordEntity>();
}

public class MedicalRecordEntity : IAuditableEntity
{
    public MedicalRecordId Id { get; set; }
    public PatientId PatientId { get; set; }
    public string Diagnosis { get; set; } = string.Empty;
    public string Treatment { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime RecordDate { get; set; }
    public string DoctorId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    public PatientEntity Patient { get; set; } = null!;
}

public interface IAuditableEntity
{
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
}
