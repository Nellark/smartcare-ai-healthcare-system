using AutoMapper;
using MediatR;
using SmartCare.Application.Common.DTOs;
using SmartCare.Domain.Repositories;

namespace SmartCare.Application.Appointments.Queries;

public record GetAllAppointmentsQuery : IRequest<ApiResponse<IReadOnlyList<AppointmentDto>>>;

public sealed class GetAllAppointmentsQueryHandler : IRequestHandler<GetAllAppointmentsQuery, ApiResponse<IReadOnlyList<AppointmentDto>>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;

    public GetAllAppointmentsQueryHandler(IAppointmentRepository appointmentRepository, IMapper mapper)
    {
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IReadOnlyList<AppointmentDto>>> Handle(GetAllAppointmentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var appointments = await _appointmentRepository.GetAllAsync(cancellationToken);
            return ApiResponse<IReadOnlyList<AppointmentDto>>.SuccessResult(_mapper.Map<IReadOnlyList<AppointmentDto>>(appointments));
        }
        catch (Exception ex)
        {
            return ApiResponse<IReadOnlyList<AppointmentDto>>.ErrorResult("An error occurred while retrieving appointments", new List<string> { ex.Message });
        }
    }
}
