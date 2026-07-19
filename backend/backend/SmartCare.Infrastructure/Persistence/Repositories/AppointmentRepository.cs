using Microsoft.EntityFrameworkCore;
using SmartCare.Domain.Entities;
using SmartCare.Domain.Repositories;
using SmartCare.Domain.ValueObjects;
using SmartCare.Infrastructure.Persistence.Entities;

namespace SmartCare.Infrastructure.Persistence.Repositories;

public sealed class AppointmentRepository : IAppointmentRepository
{
    private readonly SmartCareDbContext _context;

    public AppointmentRepository(SmartCareDbContext context)
    {
        _context = context;
    }

    public async Task<Appointment?> GetByIdAsync(AppointmentId id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Appointments
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity is null ? null : MapToDomainEntity(entity);
    }

    public async Task<IReadOnlyList<Appointment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.Appointments
            .OrderBy(x => x.ScheduledAt)
            .ToListAsync(cancellationToken);

        return entities.Select(MapToDomainEntity).ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<Appointment>> GetByPatientIdAsync(PatientId patientId, CancellationToken cancellationToken = default)
    {
        var entities = await _context.Appointments
            .Where(x => x.PatientId == patientId)
            .OrderBy(x => x.ScheduledAt)
            .ToListAsync(cancellationToken);

        return entities.Select(MapToDomainEntity).ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<Appointment>> GetByDoctorIdAsync(DoctorId doctorId, CancellationToken cancellationToken = default)
    {
        var entities = await _context.Appointments
            .Where(x => x.DoctorId == doctorId)
            .OrderBy(x => x.ScheduledAt)
            .ToListAsync(cancellationToken);

        return entities.Select(MapToDomainEntity).ToList().AsReadOnly();
    }

    public async Task AddAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        await _context.Appointments.AddAsync(MapToInfrastructureEntity(appointment), cancellationToken);
    }

    public async Task UpdateAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Appointments.FirstOrDefaultAsync(x => x.Id == appointment.Id, cancellationToken);
        if (entity is null)
        {
            return;
        }

        entity.PatientId = appointment.PatientId;
        entity.DoctorId = appointment.DoctorId;
        entity.ScheduledAt = appointment.ScheduledAt;
        entity.DurationMinutes = appointment.DurationMinutes;
        entity.Reason = appointment.Reason;
        entity.Notes = appointment.Notes;
        entity.Status = appointment.Status.ToString();
        entity.UpdatedAt = appointment.UpdatedAt;
    }

    public async Task DeleteAsync(AppointmentId id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Appointments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is not null)
        {
            _context.Appointments.Remove(entity);
        }
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    private static Appointment MapToDomainEntity(AppointmentEntity entity)
    {
        if (!Enum.TryParse<AppointmentStatus>(entity.Status, out var status))
        {
            throw new InvalidOperationException("Invalid appointment status");
        }

        return Appointment.Rehydrate(
            entity.Id,
            entity.PatientId,
            entity.DoctorId,
            entity.ScheduledAt,
            entity.DurationMinutes,
            entity.Reason,
            entity.Notes,
            status,
            entity.CreatedAt,
            entity.UpdatedAt);
    }

    private static AppointmentEntity MapToInfrastructureEntity(Appointment appointment)
    {
        return new AppointmentEntity
        {
            Id = appointment.Id,
            PatientId = appointment.PatientId,
            DoctorId = appointment.DoctorId,
            ScheduledAt = appointment.ScheduledAt,
            DurationMinutes = appointment.DurationMinutes,
            Reason = appointment.Reason,
            Notes = appointment.Notes,
            Status = appointment.Status.ToString(),
            CreatedAt = appointment.CreatedAt,
            UpdatedAt = appointment.UpdatedAt
        };
    }
}
