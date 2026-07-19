using SmartCare.Domain.Common;

namespace SmartCare.Domain.ValueObjects;

public sealed class DoctorId : ValueObject
{
    public Guid Value { get; }

    private DoctorId(Guid value)
    {
        Value = value;
    }

    public static DoctorId Create() => new(Guid.NewGuid());
    public static DoctorId FromGuid(Guid value) => new(value);

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator Guid(DoctorId doctorId) => doctorId.Value;
    public static explicit operator DoctorId(Guid value) => FromGuid(value);

    public override string ToString() => Value.ToString();
}
