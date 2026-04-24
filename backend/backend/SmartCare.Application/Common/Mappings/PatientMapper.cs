namespace SmartCare.Application.Common.Mappings;

public static class PatientMapper
{
    public static PatientDto ToDto(this Domain.Entities.Patient patient)
    {
        return new PatientDto
        {
            Id = patient.Id,
            FirstName = patient.Name.FirstName,
            LastName = patient.Name.LastName,
            Email = patient.Email,
            DateOfBirth = patient.DateOfBirth,
            PhoneNumber = patient.PhoneNumber,
            Address = patient.Address,
            Age = patient.GetAge(),
            CreatedAt = patient.CreatedAt,
            UpdatedAt = patient.UpdatedAt,
            MedicalRecords = patient.MedicalRecords.Select(mr => mr.ToDto()).ToList().AsReadOnly()
        };
    }

    public static MedicalRecordDto ToDto(this Domain.Entities.MedicalRecord medicalRecord)
    {
        return new MedicalRecordDto
        {
            Id = medicalRecord.Id,
            Diagnosis = medicalRecord.Diagnosis,
            Treatment = medicalRecord.Treatment,
            Notes = medicalRecord.Notes,
            RecordDate = medicalRecord.RecordDate,
            DoctorId = medicalRecord.DoctorId,
            CreatedAt = medicalRecord.CreatedAt,
            UpdatedAt = medicalRecord.UpdatedAt
        };
    }

    public static IReadOnlyList<PatientDto> ToDto(this IReadOnlyList<Domain.Entities.Patient> patients)
    {
        return patients.Select(p => p.ToDto()).ToList().AsReadOnly();
    }
}
