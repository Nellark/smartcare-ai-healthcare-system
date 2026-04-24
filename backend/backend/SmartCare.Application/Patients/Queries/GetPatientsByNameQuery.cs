namespace SmartCare.Application.Patients.Queries;

public record GetPatientsByNameQuery(string FirstName, string LastName) : IRequest<ApiResponse<IReadOnlyList<PatientDto>>>;

public class GetPatientsByNameQueryHandler : IRequestHandler<GetPatientsByNameQuery, ApiResponse<IReadOnlyList<PatientDto>>>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper _mapper;

    public GetPatientsByNameQueryHandler(IPatientRepository patientRepository, IMapper mapper)
    {
        _patientRepository = patientRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IReadOnlyList<PatientDto>>> Handle(GetPatientsByNameQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var patients = await _patientRepository.GetByNameAsync(request.FirstName, request.LastName, cancellationToken);
            var patientDtos = _mapper.Map<IReadOnlyList<PatientDto>>(patients);
            
            return ApiResponse<IReadOnlyList<PatientDto>>.SuccessResult(patientDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IReadOnlyList<PatientDto>>.ErrorResult("An error occurred while searching patients", new List<string> { ex.Message });
        }
    }
}
