using SmartCare.Domain.ValueObjects;

namespace SmartCare.Infrastructure.Persistence.Entities;

public class DoctorEntity : IAuditableEntity
{
    public DoctorId Id { get; set; } = default!;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
