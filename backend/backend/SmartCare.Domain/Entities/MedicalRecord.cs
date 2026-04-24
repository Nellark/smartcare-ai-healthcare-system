using SmartCare.Domain.Common;
using SmartCare.Domain.ValueObjects;

namespace SmartCare.Domain.Entities;

public sealed class MedicalRecord : Entity<MedicalRecordId>
{
    public string Diagnosis { get; private set; }
    public string Treatment { get; private set; }
    public string Notes { get; private set; }
    public DateTime RecordDate { get; private set; }
    public string DoctorId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    internal MedicalRecord(
        MedicalRecordId id,
        string diagnosis,
        string treatment,
        string notes,
        DateTime recordDate,
        string doctorId) : base(id)
    {
        Diagnosis = diagnosis;
        Treatment = treatment;
        Notes = notes;
        RecordDate = recordDate;
        DoctorId = doctorId;
        CreatedAt = DateTime.UtcNow;
    }

    public static MedicalRecord Create(
        MedicalRecordId id,
        string diagnosis,
        string treatment,
        string notes,
        DateTime recordDate,
        string doctorId)
    {
        if (string.IsNullOrWhiteSpace(diagnosis))
            throw new ArgumentException("Diagnosis cannot be empty", nameof(diagnosis));

        if (string.IsNullOrWhiteSpace(treatment))
            throw new ArgumentException("Treatment cannot be empty", nameof(treatment));

        if (string.IsNullOrWhiteSpace(doctorId))
            throw new ArgumentException("Doctor ID cannot be empty", nameof(doctorId));

        return new MedicalRecord(id, diagnosis, treatment, notes, recordDate, doctorId);
    }

    public void UpdateDetails(string diagnosis, string treatment, string notes, string doctorId)
    {
        if (string.IsNullOrWhiteSpace(diagnosis))
            throw new ArgumentException("Diagnosis cannot be empty", nameof(diagnosis));

        if (string.IsNullOrWhiteSpace(treatment))
            throw new ArgumentException("Treatment cannot be empty", nameof(treatment));

        if (string.IsNullOrWhiteSpace(doctorId))
            throw new ArgumentException("Doctor ID cannot be empty", nameof(doctorId));

        Diagnosis = diagnosis;
        Treatment = treatment;
        Notes = notes;
        DoctorId = doctorId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRecordDate(DateTime recordDate)
    {
        if (recordDate > DateTime.UtcNow)
            throw new ArgumentException("Record date cannot be in the future", nameof(recordDate));

        RecordDate = recordDate;
        UpdatedAt = DateTime.UtcNow;
    }
}
