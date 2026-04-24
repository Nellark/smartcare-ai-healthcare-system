namespace SmartCare.Domain.Services;

public interface IPatientDomainService
{
    Task<Result<bool>> CanPatientBeDeletedAsync(PatientId patientId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<Patient>>> GetPatientsWithOverlappingAppointmentsAsync(DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);
    Task<Result> ValidateMedicalRecordAccessAsync(PatientId patientId, string doctorId, CancellationToken cancellationToken = default);
    Task<Result<bool>> IsPatientEligibleForCareAsync(PatientId patientId, CancellationToken cancellationToken = default);
}
