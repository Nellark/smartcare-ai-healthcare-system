using SmartCare.Domain.ValueObjects;

namespace SmartCare.Infrastructure.Persistence.Entities;

public class AppointmentEntity : IAuditableEntity
{
    public AppointmentId Id { get; set; } = default!;
    public PatientId PatientId { get; set; } = default!;
    public DoctorId DoctorId { get; set; } = default!;
    public DateTime ScheduledAt { get; set; }
    public int DurationMinutes { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public PatientEntity Patient { get; set; } = null!;
    public DoctorEntity Doctor { get; set; } = null!;
}
