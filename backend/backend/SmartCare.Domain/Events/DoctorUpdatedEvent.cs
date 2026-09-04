using SmartCare.Domain.Common;
using SmartCare.Domain.ValueObjects;

namespace SmartCare.Domain.Events;

public sealed class DoctorUpdatedEvent : IDomainEvent
{
    public DoctorId DoctorId { get; }
    public Email OldEmail { get; }
    public Email NewEmail { get; }
    public DateTime OccurredOn { get; }

    public DoctorUpdatedEvent(DoctorId doctorId, Email oldEmail, Email newEmail)
    {
        DoctorId = doctorId;
        OldEmail = oldEmail;
        NewEmail = newEmail;
        OccurredOn = DateTime.UtcNow;
    }
}
