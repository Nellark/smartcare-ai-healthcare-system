using SmartCare.Domain.Common;

namespace SmartCare.Domain.ValueObjects;

public sealed class MedicalRecordId : ValueObject
{
    public Guid Value { get; }

    private MedicalRecordId(Guid value)
    {
        Value = value;
    }

    public static MedicalRecordId Create() => new(Guid.NewGuid());
    public static MedicalRecordId FromGuid(Guid value) => new(value);

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator Guid(MedicalRecordId medicalRecordId) => medicalRecordId.Value;
    public static explicit operator MedicalRecordId(Guid value) => FromGuid(value);

    public override string ToString() => Value.ToString();
}
