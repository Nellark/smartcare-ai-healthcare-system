using SmartCare.Domain.Common;
using SmartCare.Domain.Events;
using SmartCare.Domain.ValueObjects;

namespace SmartCare.Domain.Entities;

public sealed class Doctor : AggregateRoot<DoctorId>
{
    public FullName Name { get; private set; }
    public Email Email { get; private set; }
    public string Specialty { get; private set; }
    public string PhoneNumber { get; private set; }
    public string LicenseNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Doctor(
        DoctorId id,
        FullName name,
        Email email,
        string specialty,
        string phoneNumber,
        string licenseNumber,
        DateTime createdAt,
        DateTime? updatedAt) : base(id)
    {
        Name = name;
        Email = email;
        Specialty = specialty;
        PhoneNumber = phoneNumber;
        LicenseNumber = licenseNumber;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static Result<Doctor> Create(
        DoctorId id,
        FullName name,
        Email email,
        string specialty,
        string phoneNumber,
        string licenseNumber)
    {
        if (string.IsNullOrWhiteSpace(specialty))
            return Result<Doctor>.Failure(Error.InvalidInput);

        if (string.IsNullOrWhiteSpace(phoneNumber))
            return Result<Doctor>.Failure(Error.InvalidInput);

        if (string.IsNullOrWhiteSpace(licenseNumber))
            return Result<Doctor>.Failure(Error.InvalidInput);

        var doctor = new Doctor(id, name, email, specialty.Trim(), phoneNumber.Trim(), licenseNumber.Trim(), DateTime.UtcNow, null);
        doctor.RaiseDomainEvent(new DoctorCreatedEvent(doctor.Id, doctor.Email));

        return Result<Doctor>.Success(doctor);
    }

    public static Doctor Rehydrate(
        DoctorId id,
        FullName name,
        Email email,
        string specialty,
        string phoneNumber,
        string licenseNumber,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        return new Doctor(id, name, email, specialty, phoneNumber, licenseNumber, createdAt, updatedAt);
    }

    public Result UpdateDetails(
        FullName name,
        Email email,
        string specialty,
        string phoneNumber,
        string licenseNumber)
    {
        if (string.IsNullOrWhiteSpace(specialty))
            return Result.Failure(Error.InvalidInput);

        if (string.IsNullOrWhiteSpace(phoneNumber))
            return Result.Failure(Error.InvalidInput);

        if (string.IsNullOrWhiteSpace(licenseNumber))
            return Result.Failure(Error.InvalidInput);

        var oldEmail = Email;
        Name = name;
        Email = email;
        Specialty = specialty.Trim();
        PhoneNumber = phoneNumber.Trim();
        LicenseNumber = licenseNumber.Trim();
        UpdatedAt = DateTime.UtcNow;

        if (!oldEmail.Equals(email))
        {
            RaiseDomainEvent(new DoctorUpdatedEvent(Id, oldEmail, email));
        }

        return Result.Success();
    }
}
