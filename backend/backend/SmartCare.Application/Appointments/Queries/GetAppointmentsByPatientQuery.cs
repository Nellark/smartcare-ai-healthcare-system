using AutoMapper;
using MediatR;
using SmartCare.Application.Common.DTOs;
using SmartCare.Domain.Repositories;
using SmartCare.Domain.ValueObjects;

namespace SmartCare.Application.Appointments.Queries;

public record GetAppointmentsByPatientQuery(Guid PatientId) : IRequest<ApiResponse<IReadOnlyList<AppointmentDto>>>;

public sealed class GetAppointmentsByPatientQueryHandler : IRequestHandler<GetAppointmentsByPatientQuery, ApiResponse<IReadOnlyList<AppointmentDto>>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;

    public GetAppointmentsByPatientQueryHandler(IAppointmentRepository appointmentRepository, IMapper mapper)
    {
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IReadOnlyList<AppointmentDto>>> Handle(GetAppointmentsByPatientQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var appointments = await _appointmentRepository.GetByPatientIdAsync(PatientId.FromGuid(request.PatientId), cancellationToken);
            return ApiResponse<IReadOnlyList<AppointmentDto>>.SuccessResult(_mapper.Map<IReadOnlyList<AppointmentDto>>(appointments));
        }
        catch (Exception ex)
        {
            return ApiResponse<IReadOnlyList<AppointmentDto>>.ErrorResult("An error occurred while retrieving appointments", new List<string> { ex.Message });
        }
    }
}
