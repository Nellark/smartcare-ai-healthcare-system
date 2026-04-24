using SmartCare.Domain.Common;

namespace SmartCare.Domain.ValueObjects;

public sealed class FullName : ValueObject
{
    public string FirstName { get; }
    public string LastName { get; }

    private FullName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public static Result<FullName> Create(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result<FullName>.Failure(Error.InvalidInput);

        if (string.IsNullOrWhiteSpace(lastName))
            return Result<FullName>.Failure(Error.InvalidInput);

        if (firstName.Length > 50 || lastName.Length > 50)
            return Result<FullName>.Failure(Error.InvalidInput);

        return Result<FullName>.Success(new FullName(firstName.Trim(), lastName.Trim()));
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }

    public override string ToString() => $"{FirstName} {LastName}";
}
