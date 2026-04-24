namespace SmartCare.Application.Patients.Commands;

public record AddMedicalRecordCommand(
    Guid PatientId,
    string Diagnosis,
    string Treatment,
    string Notes,
    DateTime RecordDate,
    string DoctorId) : IRequest<ApiResponse<MedicalRecordDto>>;

public class AddMedicalRecordCommandHandler : IRequestHandler<AddMedicalRecordCommand, ApiResponse<MedicalRecordDto>>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IPatientDomainService _patientDomainService;
    private readonly IMapper _mapper;

    public AddMedicalRecordCommandHandler(
        IPatientRepository patientRepository,
        IPatientDomainService patientDomainService,
        IMapper mapper)
    {
        _patientRepository = patientRepository;
        _patientDomainService = patientDomainService;
        _mapper = mapper;
    }

    public async Task<ApiResponse<MedicalRecordDto>> Handle(AddMedicalRecordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var patientId = Domain.ValueObjects.PatientId.FromGuid(request.PatientId);
            var patient = await _patientRepository.GetByIdAsync(patientId, cancellationToken);

            if (patient == null)
            {
                return ApiResponse<MedicalRecordDto>.ErrorResult("Patient not found");
            }

            // Validate doctor access
            var accessResult = await _patientDomainService.ValidateMedicalRecordAccessAsync(patientId, request.DoctorId, cancellationToken);
            if (!accessResult.IsSuccess)
            {
                return ApiResponse<MedicalRecordDto>.ErrorResult("Access denied");
            }

            // Add medical record through aggregate
            var medicalRecordId = Domain.ValueObjects.MedicalRecordId.Create();
            var addResult = patient.AddMedicalRecord(
                medicalRecordId,
                request.Diagnosis,
                request.Treatment,
                request.Notes,
                request.RecordDate,
                request.DoctorId);

            if (!addResult.IsSuccess)
            {
                return ApiResponse<MedicalRecordDto>.ErrorResult(addResult.Error.Message);
            }

            await _patientRepository.UpdateAsync(patient, cancellationToken);
            await _patientRepository.SaveChangesAsync(cancellationToken);

            var medicalRecordDto = _mapper.Map<MedicalRecordDto>(addResult.Value);
            return ApiResponse<MedicalRecordDto>.SuccessResult(medicalRecordDto, "Medical record added successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<MedicalRecordDto>.ErrorResult("An error occurred while adding the medical record", new List<string> { ex.Message });
        }
    }
}
