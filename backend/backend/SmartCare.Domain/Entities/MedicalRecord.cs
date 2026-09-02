using SmartCare.Domain.Common;
using SmartCare.Domain.ValueObjects;

namespace SmartCare.Domain.Entities;

public sealed class MedicalRecord : Entity<MedicalRecordId>
{
    public PatientId PatientId { get; private set; }
    public string Diagnosis { get; private set; }
    public string Treatment { get; private set; }
    public string Notes { get; private set; }
    public DateTime RecordDate { get; private set; }
    public string DoctorId { get; private set; }
    public string RecordType { get; private set; }
    public string Title { get; private set; }
    public string Provider { get; private set; }
    public string Status { get; private set; }
    public string? AttachmentUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private MedicalRecord(
        MedicalRecordId id,
        PatientId patientId,
        string diagnosis,
        string treatment,
        string notes,
        DateTime recordDate,
        string doctorId,
        string recordType,
        string title,
        string provider,
        string status,
        string? attachmentUrl,
        DateTime createdAt,
        DateTime? updatedAt) : base(id)
    {
        PatientId = patientId;
        Diagnosis = diagnosis;
        Treatment = treatment;
        Notes = notes;
        RecordDate = recordDate;
        DoctorId = doctorId;
        RecordType = recordType;
        Title = title;
        Provider = provider;
        Status = status;
        AttachmentUrl = attachmentUrl;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static MedicalRecord Create(
        MedicalRecordId id,
        PatientId patientId,
        string diagnosis,
        string treatment,
        string notes,
        DateTime recordDate,
        string doctorId,
        string recordType = "CLINICAL NOTE",
        string title = "Medical Record",
        string provider = "",
        string status = "COMPLETED",
        string? attachmentUrl = null)
    {
        if (string.IsNullOrWhiteSpace(diagnosis))
            throw new ArgumentException("Diagnosis cannot be empty", nameof(diagnosis));

        if (string.IsNullOrWhiteSpace(treatment))
            throw new ArgumentException("Treatment cannot be empty", nameof(treatment));

        if (string.IsNullOrWhiteSpace(doctorId))
            throw new ArgumentException("Doctor ID cannot be empty", nameof(doctorId));

        return new MedicalRecord(id, patientId, diagnosis, treatment, notes, recordDate, doctorId, 
            recordType, title, provider, status, attachmentUrl, DateTime.UtcNow, null);
    }

    public static MedicalRecord Rehydrate(
        MedicalRecordId id,
        PatientId patientId,
        string diagnosis,
        string treatment,
        string notes,
        DateTime recordDate,
        string doctorId,
        string recordType,
        string title,
        string provider,
        string status,
        string? attachmentUrl,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        return new MedicalRecord(id, patientId, diagnosis, treatment, notes, recordDate, doctorId, 
            recordType, title, provider, status, attachmentUrl, createdAt, updatedAt);
    }

    public void UpdateDetails(string diagnosis, string treatment, string notes, string doctorId, 
        string recordType, string title, string provider, string status, string? attachmentUrl)
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
        RecordType = recordType;
        Title = title;
        Provider = provider;
        Status = status;
        
        if (attachmentUrl != null)
            AttachmentUrl = attachmentUrl;
            
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
