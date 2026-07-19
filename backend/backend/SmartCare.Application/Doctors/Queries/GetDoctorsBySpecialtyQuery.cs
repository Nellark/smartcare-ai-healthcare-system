using AutoMapper;
using MediatR;
using SmartCare.Application.Common.DTOs;
using SmartCare.Domain.Repositories;

namespace SmartCare.Application.Doctors.Queries;

public record GetDoctorsBySpecialtyQuery(string Specialty) : IRequest<ApiResponse<IReadOnlyList<DoctorDto>>>;

public sealed class GetDoctorsBySpecialtyQueryHandler : IRequestHandler<GetDoctorsBySpecialtyQuery, ApiResponse<IReadOnlyList<DoctorDto>>>
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IMapper _mapper;

    public GetDoctorsBySpecialtyQueryHandler(IDoctorRepository doctorRepository, IMapper mapper)
    {
        _doctorRepository = doctorRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IReadOnlyList<DoctorDto>>> Handle(GetDoctorsBySpecialtyQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var doctors = await _doctorRepository.GetBySpecialtyAsync(request.Specialty, cancellationToken);
            return ApiResponse<IReadOnlyList<DoctorDto>>.SuccessResult(_mapper.Map<IReadOnlyList<DoctorDto>>(doctors));
        }
        catch (Exception ex)
        {
            return ApiResponse<IReadOnlyList<DoctorDto>>.ErrorResult("An error occurred while searching doctors", new List<string> { ex.Message });
        }
    }
}
