using Microsoft.EntityFrameworkCore;
using SmartCare.Domain.Entities;
using SmartCare.Domain.Repositories;
using SmartCare.Domain.ValueObjects;
using SmartCare.Infrastructure.Persistence.Entities;

namespace SmartCare.Infrastructure.Persistence.Repositories;

public sealed class MedicalRecordRepository : IMedicalRecordRepository
{
    private readonly SmartCareDbContext _context;

    public MedicalRecordRepository(SmartCareDbContext context)
    {
        _context = context;
    }

    public async Task<MedicalRecord?> GetByIdAsync(MedicalRecordId id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.MedicalRecords
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity is null ? null : MapToDomainEntity(entity);
    }

    public async Task<IReadOnlyList<MedicalRecord>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.MedicalRecords
            .OrderByDescending(x => x.RecordDate)
            .ToListAsync(cancellationToken);

        return entities.Select(MapToDomainEntity).ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<MedicalRecord>> GetByPatientIdAsync(PatientId patientId, CancellationToken cancellationToken = default)
    {
        var entities = await _context.MedicalRecords
            .Where(x => x.PatientId == patientId)
            .OrderByDescending(x => x.RecordDate)
            .ToListAsync(cancellationToken);

        return entities.Select(MapToDomainEntity).ToList().AsReadOnly();
    }

    public async Task AddAsync(MedicalRecord medicalRecord, CancellationToken cancellationToken = default)
    {
        await _context.MedicalRecords.AddAsync(MapToInfrastructureEntity(medicalRecord), cancellationToken);
    }

    public async Task UpdateAsync(MedicalRecord medicalRecord, CancellationToken cancellationToken = default)
    {
        var entity = await _context.MedicalRecords.FirstOrDefaultAsync(x => x.Id == medicalRecord.Id, cancellationToken);
        if (entity is null)
        {
            return;
        }

        entity.PatientId = medicalRecord.PatientId;
        entity.Diagnosis = medicalRecord.Diagnosis;
        entity.Treatment = medicalRecord.Treatment;
        entity.Notes = medicalRecord.Notes;
        entity.RecordDate = medicalRecord.RecordDate;
        entity.DoctorId = medicalRecord.DoctorId;
        entity.UpdatedAt = medicalRecord.UpdatedAt;
    }

    public async Task DeleteAsync(MedicalRecordId id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.MedicalRecords.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is not null)
        {
            _context.MedicalRecords.Remove(entity);
        }
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    private static MedicalRecord MapToDomainEntity(MedicalRecordEntity entity)
    {
        return MedicalRecord.Rehydrate(
            entity.Id,
            entity.PatientId,
            entity.Diagnosis,
            entity.Treatment,
            entity.Notes,
            entity.RecordDate,
            entity.DoctorId,
            entity.CreatedAt,
            entity.UpdatedAt);
    }

    private static MedicalRecordEntity MapToInfrastructureEntity(MedicalRecord medicalRecord)
    {
        return new MedicalRecordEntity
        {
            Id = medicalRecord.Id,
            PatientId = medicalRecord.PatientId,
            Diagnosis = medicalRecord.Diagnosis,
            Treatment = medicalRecord.Treatment,
            Notes = medicalRecord.Notes,
            RecordDate = medicalRecord.RecordDate,
            DoctorId = medicalRecord.DoctorId,
            CreatedAt = medicalRecord.CreatedAt,
            UpdatedAt = medicalRecord.UpdatedAt
        };
    }
}
