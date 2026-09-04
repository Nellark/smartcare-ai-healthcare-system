using AutoMapper;
using MediatR;
using SmartCare.Application.Common.DTOs;
using SmartCare.Domain.Repositories;
using SmartCare.Domain.ValueObjects;

namespace SmartCare.Application.Appointments.Queries;

public record GetAppointmentByIdQuery(Guid Id) : IRequest<ApiResponse<AppointmentDto>>;

public sealed class GetAppointmentByIdQueryHandler : IRequestHandler<GetAppointmentByIdQuery, ApiResponse<AppointmentDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;

    public GetAppointmentByIdQueryHandler(IAppointmentRepository appointmentRepository, IMapper mapper)
    {
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<AppointmentDto>> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var appointment = await _appointmentRepository.GetByIdAsync(AppointmentId.FromGuid(request.Id), cancellationToken);
            if (appointment is null)
            {
                return ApiResponse<AppointmentDto>.ErrorResult("Appointment not found");
            }

            return ApiResponse<AppointmentDto>.SuccessResult(_mapper.Map<AppointmentDto>(appointment));
        }
        catch (Exception ex)
        {
            return ApiResponse<AppointmentDto>.ErrorResult("An error occurred while retrieving the appointment", new List<string> { ex.Message });
        }
    }
}
