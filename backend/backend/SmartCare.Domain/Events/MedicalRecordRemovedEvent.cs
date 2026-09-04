using SmartCare.Domain.Common;
using SmartCare.Domain.ValueObjects;

namespace SmartCare.Domain.Events;

public sealed class MedicalRecordRemovedEvent : IDomainEvent
{
    public PatientId PatientId { get; }
    public MedicalRecordId MedicalRecordId { get; }
    public DateTime OccurredOn { get; }

    public MedicalRecordRemovedEvent(PatientId patientId, MedicalRecordId medicalRecordId)
    {
        PatientId = patientId;
        MedicalRecordId = medicalRecordId;
        OccurredOn = DateTime.UtcNow;
    }
}
