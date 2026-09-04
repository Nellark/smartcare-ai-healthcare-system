using AutoMapper;
using MediatR;
using SmartCare.Application.Common.DTOs;
using SmartCare.Domain.Repositories;
using SmartCare.Domain.ValueObjects;

namespace SmartCare.Application.Appointments.Commands;

public record CreateAppointmentCommand(
    Guid PatientId,
    Guid DoctorId,
    DateTime ScheduledAt,
    int DurationMinutes,
    string Reason,
    string Notes,
    string Status) : IRequest<ApiResponse<AppointmentDto>>;

public sealed class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, ApiResponse<AppointmentDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IMapper _mapper;

    public CreateAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IPatientRepository patientRepository,
        IDoctorRepository doctorRepository,
        IMapper mapper)
    {
        _appointmentRepository = appointmentRepository;
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<AppointmentDto>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var patientId = PatientId.FromGuid(request.PatientId);
            if (await _patientRepository.GetByIdAsync(patientId, cancellationToken) is null)
            {
                return ApiResponse<AppointmentDto>.ErrorResult("Patient not found");
            }

            var doctorId = DoctorId.FromGuid(request.DoctorId);
            if (await _doctorRepository.GetByIdAsync(doctorId, cancellationToken) is null)
            {
                return ApiResponse<AppointmentDto>.ErrorResult("Doctor not found");
            }

            if (!Enum.TryParse<Domain.Entities.AppointmentStatus>(request.Status, true, out var status))
            {
                return ApiResponse<AppointmentDto>.ErrorResult("Invalid appointment status");
            }

            var appointmentResult = Domain.Entities.Appointment.Create(
                AppointmentId.Create(),
                patientId,
                doctorId,
                request.ScheduledAt,
                request.DurationMinutes,
                request.Reason,
                request.Notes,
                status);

            if (!appointmentResult.IsSuccess)
            {
                return ApiResponse<AppointmentDto>.ErrorResult(appointmentResult.Error.Message);
            }

            var appointment = appointmentResult.Value;
            await _appointmentRepository.AddAsync(appointment, cancellationToken);
            await _appointmentRepository.SaveChangesAsync(cancellationToken);

            return ApiResponse<AppointmentDto>.SuccessResult(_mapper.Map<AppointmentDto>(appointment), "Appointment created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<AppointmentDto>.ErrorResult("An error occurred while creating the appointment", new List<string> { ex.Message });
        }
    }
}
