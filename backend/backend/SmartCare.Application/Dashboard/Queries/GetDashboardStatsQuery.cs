using AutoMapper;
using MediatR;
using SmartCare.Application.Common.DTOs;
using SmartCare.Domain.Repositories;

namespace SmartCare.Application.Dashboard.Queries;

public record GetDashboardStatsQuery : IRequest<ApiResponse<DashboardStatsDto>>;

public sealed class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, ApiResponse<DashboardStatsDto>>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;

    public GetDashboardStatsQueryHandler(
        IPatientRepository patientRepository,
        IDoctorRepository doctorRepository,
        IAppointmentRepository appointmentRepository,
        IMapper mapper)
    {
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<DashboardStatsDto>> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var patients = await _patientRepository.GetAllAsync(cancellationToken);
            var doctors = await _doctorRepository.GetAllAsync(cancellationToken);
            var appointments = await _appointmentRepository.GetAllAsync(cancellationToken);

            var stats = new DashboardStatsDto
            {
                TotalPatients = patients.Count,
                TotalDoctors = doctors.Count,
                TotalAppointments = appointments.Count,
                RecentPatients = _mapper.Map<IReadOnlyList<PatientDto>>(patients.OrderByDescending(x => x.CreatedAt).Take(5).ToList()),
                RecentAppointments = _mapper.Map<IReadOnlyList<AppointmentDto>>(appointments.OrderByDescending(x => x.ScheduledAt).Take(5).ToList())
            };

            return ApiResponse<DashboardStatsDto>.SuccessResult(stats);
        }
        catch (Exception ex)
        {
            return ApiResponse<DashboardStatsDto>.ErrorResult("An error occurred while retrieving dashboard statistics", new List<string> { ex.Message });
        }
    }
}
