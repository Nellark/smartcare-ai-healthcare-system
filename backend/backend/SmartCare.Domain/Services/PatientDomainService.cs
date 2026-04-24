using SmartCare.Domain.Common;
using SmartCare.Domain.Entities;
using SmartCare.Domain.Repositories;
using SmartCare.Domain.ValueObjects;

namespace SmartCare.Domain.Services;

public class PatientDomainService : IPatientDomainService
{
    private readonly IPatientRepository _patientRepository;

    public PatientDomainService(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<Result<bool>> CanPatientBeDeletedAsync(PatientId patientId, CancellationToken cancellationToken = default)
    {
        var patient = await _patientRepository.GetByIdAsync(patientId, cancellationToken);
        if (patient == null)
            return Result<bool>.Failure(Error.NotFound);

        var hasActiveMedicalRecords = patient.MedicalRecords.Any();
        if (hasActiveMedicalRecords)
        {
            return Result<bool>.Failure(Error.InvalidInput);
        }

        return Result<bool>.Success(true);
    }

    public async Task<Result<IReadOnlyList<Patient>>> GetPatientsWithOverlappingAppointmentsAsync(DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default)
    {
        if (startTime >= endTime)
            return Result<IReadOnlyList<Patient>>.Failure(Error.InvalidInput);

        var allPatients = await _patientRepository.GetAllAsync(cancellationToken);
        
        var patientsWithOverlappingAppointments = allPatients
            .Where(p => HasOverlappingMedicalRecords(p, startTime, endTime))
            .ToList();

        return Result<IReadOnlyList<Patient>>.Success(patientsWithOverlappingAppointments.AsReadOnly());
    }

    public async Task<Result> ValidateMedicalRecordAccessAsync(PatientId patientId, string doctorId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(doctorId))
            return Result.Failure(Error.Unauthorized);

        var patient = await _patientRepository.GetByIdAsync(patientId, cancellationToken);
        if (patient == null)
            return Result.Failure(Error.NotFound);

        return Result.Success();
    }

    public async Task<Result<bool>> IsPatientEligibleForCareAsync(PatientId patientId, CancellationToken cancellationToken = default)
    {
        var patient = await _patientRepository.GetByIdAsync(patientId, cancellationToken);
        if (patient == null)
            return Result<bool>.Failure(Error.NotFound);

        var age = patient.GetAge();
        
        if (age < 0 || age > 150)
            return Result<bool>.Failure(Error.InvalidInput);

        return Result<bool>.Success(true);
    }

    private static bool HasOverlappingMedicalRecords(Patient patient, DateTime startTime, DateTime endTime)
    {
        return patient.MedicalRecords
            .Any(mr => mr.RecordDate >= startTime && mr.RecordDate <= endTime);
    }
}
