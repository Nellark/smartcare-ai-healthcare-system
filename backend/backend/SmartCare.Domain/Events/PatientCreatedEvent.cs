using SmartCare.Domain.Common;
using SmartCare.Domain.ValueObjects;

namespace SmartCare.Domain.Events;

public sealed class PatientCreatedEvent : IDomainEvent
{
    public PatientId PatientId { get; }
    public Email Email { get; }
    public DateTime OccurredOn { get; }

    public PatientCreatedEvent(PatientId patientId, Email email)
    {
        PatientId = patientId;
        Email = email;
        OccurredOn = DateTime.UtcNow;
    }
}
