using SmartCare.Domain.Common;
using SmartCare.Domain.ValueObjects;

namespace SmartCare.Domain.Events;

public sealed class DoctorCreatedEvent : IDomainEvent
{
    public DoctorId DoctorId { get; }
    public Email Email { get; }
    public DateTime OccurredOn { get; }

    public DoctorCreatedEvent(DoctorId doctorId, Email email)
    {
        DoctorId = doctorId;
        Email = email;
        OccurredOn = DateTime.UtcNow;
    }
}
