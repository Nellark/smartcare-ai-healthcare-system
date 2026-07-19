using MediatR;
using SmartCare.Application.Common.DTOs;
using SmartCare.Domain.Repositories;
using AutoMapper;

namespace SmartCare.Application.Patients.Queries;

public record GetPatientsBySearchQuery(string SearchTerm) : IRequest<ApiResponse<IReadOnlyList<PatientDto>>>;

public class GetPatientsBySearchQueryHandler : IRequestHandler<GetPatientsBySearchQuery, ApiResponse<IReadOnlyList<PatientDto>>>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper _mapper;

    public GetPatientsBySearchQueryHandler(IPatientRepository patientRepository, IMapper mapper)
    {
        _patientRepository = patientRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IReadOnlyList<PatientDto>>> Handle(GetPatientsBySearchQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var patients = await _patientRepository.SearchAsync(request.SearchTerm, cancellationToken);
            var patientDtos = _mapper.Map<IReadOnlyList<PatientDto>>(patients);
            
            return ApiResponse<IReadOnlyList<PatientDto>>.SuccessResult(patientDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IReadOnlyList<PatientDto>>.ErrorResult("An error occurred while searching patients", new List<string> { ex.Message });
        }
    }
}
