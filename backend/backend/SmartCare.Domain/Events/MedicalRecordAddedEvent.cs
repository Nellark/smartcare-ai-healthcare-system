namespace SmartCare.Domain.Events;

public sealed class MedicalRecordAddedEvent : IDomainEvent
{
    public PatientId PatientId { get; }
    public MedicalRecordId MedicalRecordId { get; }
    public string DoctorId { get; }
    public DateTime OccurredOn { get; }

    public MedicalRecordAddedEvent(PatientId patientId, MedicalRecordId medicalRecordId, string doctorId)
    {
        PatientId = patientId;
        MedicalRecordId = medicalRecordId;
        DoctorId = doctorId;
        OccurredOn = DateTime.UtcNow;
    }
}
