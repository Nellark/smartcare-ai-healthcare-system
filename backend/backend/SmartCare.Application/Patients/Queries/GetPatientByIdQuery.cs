namespace SmartCare.Application.Patients.Queries;

public record GetPatientByIdQuery(Guid Id) : IRequest<ApiResponse<PatientDto>>;

public class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, ApiResponse<PatientDto>>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper _mapper;

    public GetPatientByIdQueryHandler(IPatientRepository patientRepository, IMapper mapper)
    {
        _patientRepository = patientRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PatientDto>> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var patientId = Domain.ValueObjects.PatientId.FromGuid(request.Id);
            var patient = await _patientRepository.GetByIdAsync(patientId, cancellationToken);

            if (patient == null)
            {
                return ApiResponse<PatientDto>.ErrorResult("Patient not found");
            }

            var patientDto = _mapper.Map<PatientDto>(patient);
            return ApiResponse<PatientDto>.SuccessResult(patientDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<PatientDto>.ErrorResult("An error occurred while retrieving the patient", new List<string> { ex.Message });
        }
    }
}
