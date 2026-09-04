using SmartCare.Domain.Entities;
using SmartCare.Domain.ValueObjects;

namespace SmartCare.Domain.Repositories;

public interface IMedicalRecordRepository
{
    Task<MedicalRecord?> GetByIdAsync(MedicalRecordId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MedicalRecord>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MedicalRecord>> GetByPatientIdAsync(PatientId patientId, CancellationToken cancellationToken = default);
    Task AddAsync(MedicalRecord medicalRecord, CancellationToken cancellationToken = default);
    Task UpdateAsync(MedicalRecord medicalRecord, CancellationToken cancellationToken = default);
    Task DeleteAsync(MedicalRecordId id, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
