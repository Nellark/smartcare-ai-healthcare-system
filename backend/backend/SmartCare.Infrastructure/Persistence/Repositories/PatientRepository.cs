namespace SmartCare.Infrastructure.Persistence.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly SmartCareDbContext _context;
    private readonly IMapper _mapper;

    public PatientRepository(SmartCareDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
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

    public async Task<IReadOnlyList<Domain.Entities.Patient>> GetByNameAsync(string firstName, string lastName, CancellationToken cancellationToken = default)
    {
        var patientEntities = await _context.Patients
            .Include(p => p.MedicalRecords)
            .Where(p => p.FirstName == firstName && p.LastName == lastName)
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

        var patient = Domain.Entities.Patient.Create(
            entity.Id,
            nameResult.Value,
            emailResult.Value,
            entity.DateOfBirth,
            entity.PhoneNumber,
            entity.Address).Value;

        // Add medical records
        foreach (var medicalRecordEntity in entity.MedicalRecords)
        {
            var medicalRecord = Domain.Entities.MedicalRecord.Create(
                medicalRecordEntity.Id,
                medicalRecordEntity.Diagnosis,
                medicalRecordEntity.Treatment,
                medicalRecordEntity.Notes,
                medicalRecordEntity.RecordDate,
                medicalRecordEntity.DoctorId);

            // Use reflection to add medical record since AddMedicalRecord raises events
            var medicalRecordsField = typeof(Domain.Entities.Patient)
                .GetField("_medicalRecords", BindingFlags.NonPublic | BindingFlags.Instance);
            
            if (medicalRecordsField?.GetValue(patient) is List<Domain.Entities.MedicalRecord> medicalRecords)
            {
                medicalRecords.Add(medicalRecord);
            }
        }

        return patient;
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
