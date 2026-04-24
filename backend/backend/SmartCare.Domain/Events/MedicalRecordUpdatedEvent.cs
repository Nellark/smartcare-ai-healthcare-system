namespace SmartCare.Domain.Events;

public sealed class MedicalRecordUpdatedEvent : IDomainEvent
{
    public PatientId PatientId { get; }
    public MedicalRecordId MedicalRecordId { get; }
    public string DoctorId { get; }
    public DateTime OccurredOn { get; }

    public MedicalRecordUpdatedEvent(PatientId patientId, MedicalRecordId medicalRecordId, string doctorId)
    {
        PatientId = patientId;
        MedicalRecordId = medicalRecordId;
        DoctorId = doctorId;
        OccurredOn = DateTime.UtcNow;
    }
}
