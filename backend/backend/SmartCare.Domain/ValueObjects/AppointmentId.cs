using SmartCare.Domain.Common;

namespace SmartCare.Domain.ValueObjects;

public sealed class AppointmentId : ValueObject
{
    public Guid Value { get; }

    private AppointmentId(Guid value)
    {
        Value = value;
    }

    public static AppointmentId Create() => new(Guid.NewGuid());
    public static AppointmentId FromGuid(Guid value) => new(value);

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator Guid(AppointmentId appointmentId) => appointmentId.Value;
    public static explicit operator AppointmentId(Guid value) => FromGuid(value);

    public override string ToString() => Value.ToString();
}
