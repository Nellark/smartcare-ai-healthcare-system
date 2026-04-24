using SmartCare.Domain.Common;

namespace SmartCare.Domain.ValueObjects;

public sealed class PatientId : ValueObject
{
    public Guid Value { get; }

    private PatientId(Guid value)
    {
        Value = value;
    }

    public static PatientId Create() => new(Guid.NewGuid());
    public static PatientId FromGuid(Guid value) => new(value);

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator Guid(PatientId patientId) => patientId.Value;
    public static explicit operator PatientId(Guid value) => FromGuid(value);

    public override string ToString() => Value.ToString();
}
