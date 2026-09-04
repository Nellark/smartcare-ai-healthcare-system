using SmartCare.Domain.Common;
using SmartCare.Domain.ValueObjects;

namespace SmartCare.Domain.Entities;

public sealed class Appointment : AggregateRoot<AppointmentId>
{
    public PatientId PatientId { get; private set; }
    public DoctorId DoctorId { get; private set; }
    public DateTime ScheduledAt { get; private set; }
    public int DurationMinutes { get; private set; }
    public string Reason { get; private set; }
    public string Notes { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Appointment(
        AppointmentId id,
        PatientId patientId,
        DoctorId doctorId,
        DateTime scheduledAt,
        int durationMinutes,
        string reason,
        string notes,
        AppointmentStatus status,
        DateTime createdAt,
        DateTime? updatedAt) : base(id)
    {
        PatientId = patientId;
        DoctorId = doctorId;
        ScheduledAt = scheduledAt;
        DurationMinutes = durationMinutes;
        Reason = reason;
        Notes = notes;
        Status = status;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static Result<Appointment> Create(
        AppointmentId id,
        PatientId patientId,
        DoctorId doctorId,
        DateTime scheduledAt,
        int durationMinutes,
        string reason,
        string notes,
        AppointmentStatus status)
    {
        if (scheduledAt == default)
            return Result<Appointment>.Failure(Error.InvalidInput);

        if (durationMinutes <= 0)
            return Result<Appointment>.Failure(Error.InvalidInput);

        if (string.IsNullOrWhiteSpace(reason))
            return Result<Appointment>.Failure(Error.InvalidInput);

        if (!Enum.IsDefined(typeof(AppointmentStatus), status))
            return Result<Appointment>.Failure(Error.InvalidInput);

        return Result<Appointment>.Success(new Appointment(
            id,
            patientId,
            doctorId,
            scheduledAt,
            durationMinutes,
            reason.Trim(),
            notes?.Trim() ?? string.Empty,
            status,
            DateTime.UtcNow,
            null));
    }

    public static Appointment Rehydrate(
        AppointmentId id,
        PatientId patientId,
        DoctorId doctorId,
        DateTime scheduledAt,
        int durationMinutes,
        string reason,
        string notes,
        AppointmentStatus status,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        return new Appointment(id, patientId, doctorId, scheduledAt, durationMinutes, reason, notes, status, createdAt, updatedAt);
    }

    public Result UpdateDetails(
        PatientId patientId,
        DoctorId doctorId,
        DateTime scheduledAt,
        int durationMinutes,
        string reason,
        string notes,
        AppointmentStatus status)
    {
        if (scheduledAt == default)
            return Result.Failure(Error.InvalidInput);

        if (durationMinutes <= 0)
            return Result.Failure(Error.InvalidInput);

        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure(Error.InvalidInput);

        if (!Enum.IsDefined(typeof(AppointmentStatus), status))
            return Result.Failure(Error.InvalidInput);

        PatientId = patientId;
        DoctorId = doctorId;
        ScheduledAt = scheduledAt;
        DurationMinutes = durationMinutes;
        Reason = reason.Trim();
        Notes = notes?.Trim() ?? string.Empty;
        Status = status;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }
}

public enum AppointmentStatus
{
    Scheduled = 0,
    Confirmed = 1,
    Completed = 2,
    Cancelled = 3,
    NoShow = 4
}
