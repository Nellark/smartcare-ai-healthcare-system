namespace SmartCare.Domain.Entities;

public sealed class Patient : AggregateRoot<PatientId>
{
    public FullName Name { get; private set; }
    public Email Email { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public string PhoneNumber { get; private set; }
    public string Address { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private readonly List<MedicalRecord> _medicalRecords = new();
    public IReadOnlyCollection<MedicalRecord> MedicalRecords => _medicalRecords.AsReadOnly();

    private Patient(
        PatientId id,
        FullName name,
        Email email,
        DateTime dateOfBirth,
        string phoneNumber,
        string address) : base(id)
    {
        Name = name;
        Email = email;
        DateOfBirth = dateOfBirth;
        PhoneNumber = phoneNumber;
        Address = address;
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<Patient> Create(
        PatientId id,
        FullName name,
        Email email,
        DateTime dateOfBirth,
        string phoneNumber,
        string address)
    {
        if (dateOfBirth > DateTime.Today)
            return Result<Patient>.Failure(Error.InvalidInput);

        if (string.IsNullOrWhiteSpace(phoneNumber))
            return Result<Patient>.Failure(Error.InvalidInput);

        if (string.IsNullOrWhiteSpace(address))
            return Result<Patient>.Failure(Error.InvalidInput);

        var patient = new Patient(id, name, email, dateOfBirth, phoneNumber, address);
        
        patient.RaiseDomainEvent(new PatientCreatedEvent(patient.Id, patient.Email));
        
        return Result<Patient>.Success(patient);
    }

    public Result<MedicalRecord> AddMedicalRecord(
        MedicalRecordId recordId,
        string diagnosis,
        string treatment,
        string notes,
        DateTime recordDate,
        string doctorId)
    {
        if (string.IsNullOrWhiteSpace(doctorId))
            return Result<MedicalRecord>.Failure(Error.InvalidInput);

        var medicalRecord = MedicalRecord.Create(recordId, diagnosis, treatment, notes, recordDate, doctorId);
        _medicalRecords.Add(medicalRecord);
        
        UpdatedAt = DateTime.UtcNow;
        
        RaiseDomainEvent(new MedicalRecordAddedEvent(Id, medicalRecord.Id, doctorId));
        
        return Result<MedicalRecord>.Success(medicalRecord);
    }

    public Result<MedicalRecord> UpdateMedicalRecord(
        MedicalRecordId recordId,
        string diagnosis,
        string treatment,
        string notes,
        string doctorId)
    {
        var medicalRecord = _medicalRecords.FirstOrDefault(mr => mr.Id == recordId);
        if (medicalRecord == null)
            return Result<MedicalRecord>.Failure(Error.NotFound);

        medicalRecord.UpdateDetails(diagnosis, treatment, notes, doctorId);
        UpdatedAt = DateTime.UtcNow;
        
        RaiseDomainEvent(new MedicalRecordUpdatedEvent(Id, medicalRecord.Id, doctorId));
        
        return Result<MedicalRecord>.Success(medicalRecord);
    }

    public Result RemoveMedicalRecord(MedicalRecordId recordId)
    {
        var medicalRecord = _medicalRecords.FirstOrDefault(mr => mr.Id == recordId);
        if (medicalRecord == null)
            return Result.Failure(Error.NotFound);

        _medicalRecords.Remove(medicalRecord);
        UpdatedAt = DateTime.UtcNow;
        
        RaiseDomainEvent(new MedicalRecordRemovedEvent(Id, medicalRecord.Id));
        
        return Result.Success();
    }

    public Result<MedicalRecord> GetMedicalRecord(MedicalRecordId recordId)
    {
        var medicalRecord = _medicalRecords.FirstOrDefault(mr => mr.Id == recordId);
        return medicalRecord != null 
            ? Result<MedicalRecord>.Success(medicalRecord) 
            : Result<MedicalRecord>.Failure(Error.NotFound);
    }

    public IReadOnlyCollection<MedicalRecord> GetMedicalRecordsByDateRange(DateTime startDate, DateTime endDate)
    {
        return _medicalRecords
            .Where(mr => mr.RecordDate >= startDate && mr.RecordDate <= endDate)
            .ToList()
            .AsReadOnly();
    }

    public Result UpdatePersonalInformation(FullName name, Email email, string phoneNumber, string address)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return Result.Failure(Error.InvalidInput);

        if (string.IsNullOrWhiteSpace(address))
            return Result.Failure(Error.InvalidInput);

        var oldEmail = Email;
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;
        UpdatedAt = DateTime.UtcNow;

        if (!oldEmail.Equals(email))
        {
            RaiseDomainEvent(new PatientEmailUpdatedEvent(Id, oldEmail, email));
        }

        return Result.Success();
    }

    public int GetAge()
    {
        var today = DateTime.Today;
        var age = today.Year - DateOfBirth.Year;
        if (DateOfBirth.Date > today.AddYears(-age)) age--;
        return age;
    }
}
