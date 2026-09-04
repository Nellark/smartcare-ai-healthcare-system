using SmartCare.Domain.Common;
using SmartCare.Domain.ValueObjects;

namespace SmartCare.Domain.Events;

public sealed class PatientEmailUpdatedEvent : IDomainEvent
{
    public PatientId PatientId { get; }
    public Email OldEmail { get; }
    public Email NewEmail { get; }
    public DateTime OccurredOn { get; }

    public PatientEmailUpdatedEvent(PatientId patientId, Email oldEmail, Email newEmail)
    {
        PatientId = patientId;
        OldEmail = oldEmail;
        NewEmail = newEmail;
        OccurredOn = DateTime.UtcNow;
    }
}
