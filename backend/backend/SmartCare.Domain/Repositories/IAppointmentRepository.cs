using SmartCare.Domain.Entities;
using SmartCare.Domain.ValueObjects;

namespace SmartCare.Domain.Repositories;

public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(AppointmentId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Appointment>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Appointment>> GetByPatientIdAsync(PatientId patientId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Appointment>> GetByDoctorIdAsync(DoctorId doctorId, CancellationToken cancellationToken = default);
    Task AddAsync(Appointment appointment, CancellationToken cancellationToken = default);
    Task UpdateAsync(Appointment appointment, CancellationToken cancellationToken = default);
    Task DeleteAsync(AppointmentId id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
