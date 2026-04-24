using MediatR;
using SmartCare.Application.Common.DTOs;
using SmartCare.Domain.Repositories;
using AutoMapper;

namespace SmartCare.Application.Patients.Queries;

public record GetAllPatientsQuery : IRequest<ApiResponse<IReadOnlyList<PatientDto>>>;

public class GetAllPatientsQueryHandler : IRequestHandler<GetAllPatientsQuery, ApiResponse<IReadOnlyList<PatientDto>>>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper _mapper;

    public GetAllPatientsQueryHandler(IPatientRepository patientRepository, IMapper mapper)
    {
        _patientRepository = patientRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IReadOnlyList<PatientDto>>> Handle(GetAllPatientsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var patients = await _patientRepository.GetAllAsync(cancellationToken);
            var patientDtos = _mapper.Map<IReadOnlyList<PatientDto>>(patients);
            
            return ApiResponse<IReadOnlyList<PatientDto>>.SuccessResult(patientDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IReadOnlyList<PatientDto>>.ErrorResult("An error occurred while retrieving patients", new List<string> { ex.Message });
        }
    }
}
