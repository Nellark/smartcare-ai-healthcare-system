using SmartCare.Domain.Repositories;
using SmartCare.Domain.ValueObjects;
using SmartCare.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace SmartCare.Infrastructure.Persistence.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly SmartCareDbContext _context;

    public PatientRepository(SmartCareDbContext context)
    {
        _context = context;
    }

    public async Task<Domain.Entities.Patient?> GetByIdAsync(PatientId id, CancellationToken cancellationToken = default)
    {
        var patientEntity = await _context.Patients
            .Include(p => p.MedicalRecords)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        return patientEntity != null ? MapToDomainEntity(patientEntity) : null;
    }

    public async Task<IReadOnlyList<Domain.Entities.Patient>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var patientEntities = await _context.Patients
            .Include(p => p.MedicalRecords)
            .ToListAsync(cancellationToken);

        return patientEntities.Select(MapToDomainEntity).ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<Domain.Entities.Patient>> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        var patientEntities = await _context.Patients
            .Include(p => p.MedicalRecords)
            .Where(p => p.Email == email.Value)
            .ToListAsync(cancellationToken);

        return patientEntities.Select(MapToDomainEntity).ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<Domain.Entities.Patient>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllAsync(cancellationToken);
        }

        var normalizedTerm = searchTerm.Trim();
        var patientEntities = await _context.Patients
            .Include(p => p.MedicalRecords)
            .Where(p =>
                EF.Functions.Like(p.FirstName, $"%{normalizedTerm}%") ||
                EF.Functions.Like(p.LastName, $"%{normalizedTerm}%") ||
                EF.Functions.Like(p.Email, $"%{normalizedTerm}%") ||
                EF.Functions.Like(p.PhoneNumber, $"%{normalizedTerm}%") ||
                EF.Functions.Like(p.FirstName + " " + p.LastName, $"%{normalizedTerm}%"))
            .ToListAsync(cancellationToken);

        return patientEntities.Select(MapToDomainEntity).ToList().AsReadOnly();
    }

    public async Task<bool> ExistsAsync(PatientId id, CancellationToken cancellationToken = default)
    {
        return await _context.Patients.AnyAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _context.Patients.AnyAsync(p => p.Email == email.Value, cancellationToken);
    }

    public async Task AddAsync(Domain.Entities.Patient patient, CancellationToken cancellationToken = default)
    {
        var patientEntity = MapToInfrastructureEntity(patient);
        await _context.Patients.AddAsync(patientEntity, cancellationToken);
    }

    public async Task UpdateAsync(Domain.Entities.Patient patient, CancellationToken cancellationToken = default)
    {
        var existingEntity = await _context.Patients
            .Include(p => p.MedicalRecords)
            .FirstOrDefaultAsync(p => p.Id == patient.Id, cancellationToken);

        if (existingEntity != null)
        {
            UpdatePatientEntity(existingEntity, patient);
            
            // Handle medical records
            SyncMedicalRecords(existingEntity, patient.MedicalRecords);
        }
    }

    public async Task DeleteAsync(PatientId id, CancellationToken cancellationToken = default)
    {
        var patientEntity = await _context.Patients
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (patientEntity != null)
        {
            _context.Patients.Remove(patientEntity);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    private static Domain.Entities.Patient MapToDomainEntity(PatientEntity entity)
    {
        var nameResult = Domain.ValueObjects.FullName.Create(entity.FirstName, entity.LastName);
        var emailResult = Domain.ValueObjects.Email.Create(entity.Email);
        
        if (!nameResult.IsSuccess || !emailResult.IsSuccess)
        {
            throw new InvalidOperationException("Invalid entity data");
        }

        var medicalRecords = entity.MedicalRecords
            .Select(medicalRecordEntity =>
            {
                var medicalRecord = Domain.Entities.MedicalRecord.Rehydrate(
                    medicalRecordEntity.Id,
                    medicalRecordEntity.PatientId,
                    medicalRecordEntity.Diagnosis,
                    medicalRecordEntity.Treatment,
                    medicalRecordEntity.Notes,
                    medicalRecordEntity.RecordDate,
                    medicalRecordEntity.DoctorId,
                    medicalRecordEntity.CreatedAt,
                    medicalRecordEntity.UpdatedAt);

                return medicalRecord;
            })
            .ToList();

        return Domain.Entities.Patient.Rehydrate(
            entity.Id,
            nameResult.Value,
            emailResult.Value,
            entity.DateOfBirth,
            entity.PhoneNumber,
            entity.Address,
            entity.Gender,
            entity.CreatedAt,
            entity.UpdatedAt,
            medicalRecords);
    }

    private static PatientEntity MapToInfrastructureEntity(Domain.Entities.Patient patient)
    {
        return new PatientEntity
        {
            Id = patient.Id,
            FirstName = patient.Name.FirstName,
            LastName = patient.Name.LastName,
            Email = patient.Email,
            DateOfBirth = patient.DateOfBirth,
            PhoneNumber = patient.PhoneNumber,
            Address = patient.Address,
            Gender = patient.Gender,
            CreatedAt = patient.CreatedAt,
            UpdatedAt = patient.UpdatedAt,
            MedicalRecords = patient.MedicalRecords.Select(mr => new MedicalRecordEntity
            {
                Id = mr.Id,
                PatientId = patient.Id,
                Diagnosis = mr.Diagnosis,
                Treatment = mr.Treatment,
                Notes = mr.Notes,
                RecordDate = mr.RecordDate,
                DoctorId = mr.DoctorId,
                CreatedAt = mr.CreatedAt,
                UpdatedAt = mr.UpdatedAt
            }).ToList()
        };
    }

    private static void UpdatePatientEntity(PatientEntity entity, Domain.Entities.Patient patient)
    {
        entity.FirstName = patient.Name.FirstName;
        entity.LastName = patient.Name.LastName;
        entity.Email = patient.Email;
        entity.DateOfBirth = patient.DateOfBirth;
        entity.PhoneNumber = patient.PhoneNumber;
        entity.Address = patient.Address;
        entity.Gender = patient.Gender;
        entity.UpdatedAt = patient.UpdatedAt;
    }

    private void SyncMedicalRecords(PatientEntity patientEntity, IReadOnlyCollection<Domain.Entities.MedicalRecord> domainMedicalRecords)
    {
        // Remove medical records that no longer exist
        var medicalRecordsToRemove = patientEntity.MedicalRecords
            .Where(mr => !domainMedicalRecords.Any(dmr => dmr.Id == mr.Id))
            .ToList();

        foreach (var medicalRecordToRemove in medicalRecordsToRemove)
        {
            _context.MedicalRecords.Remove(medicalRecordToRemove);
        }

        // Add or update medical records
        foreach (var domainMedicalRecord in domainMedicalRecords)
        {
            var existingEntity = patientEntity.MedicalRecords
                .FirstOrDefault(mr => mr.Id == domainMedicalRecord.Id);

            if (existingEntity != null)
            {
                // Update existing
                existingEntity.Diagnosis = domainMedicalRecord.Diagnosis;
                existingEntity.Treatment = domainMedicalRecord.Treatment;
                existingEntity.Notes = domainMedicalRecord.Notes;
                existingEntity.RecordDate = domainMedicalRecord.RecordDate;
                existingEntity.DoctorId = domainMedicalRecord.DoctorId;
                existingEntity.UpdatedAt = domainMedicalRecord.UpdatedAt;
            }
            else
            {
                // Add new
                var newEntity = new MedicalRecordEntity
                {
                    Id = domainMedicalRecord.Id,
                    PatientId = patientEntity.Id,
                    Diagnosis = domainMedicalRecord.Diagnosis,
                    Treatment = domainMedicalRecord.Treatment,
                    Notes = domainMedicalRecord.Notes,
                    RecordDate = domainMedicalRecord.RecordDate,
                    DoctorId = domainMedicalRecord.DoctorId,
                    CreatedAt = domainMedicalRecord.CreatedAt,
                    UpdatedAt = domainMedicalRecord.UpdatedAt
                };
                
                patientEntity.MedicalRecords.Add(newEntity);
            }
        }
    }
}
