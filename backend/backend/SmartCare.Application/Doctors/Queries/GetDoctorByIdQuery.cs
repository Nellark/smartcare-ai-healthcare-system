using AutoMapper;
using MediatR;
using SmartCare.Application.Common.DTOs;
using SmartCare.Domain.Repositories;
using SmartCare.Domain.ValueObjects;

namespace SmartCare.Application.Doctors.Queries;

public record GetDoctorByIdQuery(Guid Id) : IRequest<ApiResponse<DoctorDto>>;

public sealed class GetDoctorByIdQueryHandler : IRequestHandler<GetDoctorByIdQuery, ApiResponse<DoctorDto>>
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IMapper _mapper;

    public GetDoctorByIdQueryHandler(IDoctorRepository doctorRepository, IMapper mapper)
    {
        _doctorRepository = doctorRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<DoctorDto>> Handle(GetDoctorByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var doctor = await _doctorRepository.GetByIdAsync(DoctorId.FromGuid(request.Id), cancellationToken);
            if (doctor is null)
            {
                return ApiResponse<DoctorDto>.ErrorResult("Doctor not found");
            }

            return ApiResponse<DoctorDto>.SuccessResult(_mapper.Map<DoctorDto>(doctor));
        }
        catch (Exception ex)
        {
            return ApiResponse<DoctorDto>.ErrorResult("An error occurred while retrieving the doctor", new List<string> { ex.Message });
        }
    }
}
