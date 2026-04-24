namespace SmartCare.Domain.Repositories;

public interface IPatientRepository
{
    Task<Patient?> GetByIdAsync(PatientId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Patient>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Patient>> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Patient>> GetByNameAsync(string firstName, string lastName, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(PatientId id, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(Email email, CancellationToken cancellationToken = default);
    Task AddAsync(Patient patient, CancellationToken cancellationToken = default);
    Task UpdateAsync(Patient patient, CancellationToken cancellationToken = default);
    Task DeleteAsync(PatientId id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
