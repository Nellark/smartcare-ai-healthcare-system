using SmartCare.Domain.Entities;
using SmartCare.Domain.ValueObjects;

namespace SmartCare.Domain.Repositories;

public interface IDoctorRepository
{
    Task<Doctor?> GetByIdAsync(DoctorId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Doctor>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Doctor>> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Doctor>> GetByNameAsync(string firstName, string lastName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Doctor>> GetBySpecialtyAsync(string specialty, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(DoctorId id, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(Email email, CancellationToken cancellationToken = default);
    Task<bool> LicenseNumberExistsAsync(string licenseNumber, CancellationToken cancellationToken = default);
    Task AddAsync(Doctor doctor, CancellationToken cancellationToken = default);
    Task UpdateAsync(Doctor doctor, CancellationToken cancellationToken = default);
    Task DeleteAsync(DoctorId id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
