using AutoMapper;
using MediatR;
using SmartCare.Application.Common.DTOs;
using SmartCare.Domain.Repositories;
using SmartCare.Domain.ValueObjects;

namespace SmartCare.Application.Appointments.Commands;

public record UpdateAppointmentCommand(
    Guid Id,
    Guid PatientId,
    Guid DoctorId,
    DateTime ScheduledAt,
    int DurationMinutes,
    string Reason,
    string Notes,
    string Status) : IRequest<ApiResponse<AppointmentDto>>;

public sealed class UpdateAppointmentCommandHandler : IRequestHandler<UpdateAppointmentCommand, ApiResponse<AppointmentDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IMapper _mapper;

    public UpdateAppointmentCommandHandler(
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

    public async Task<ApiResponse<AppointmentDto>> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var appointmentId = AppointmentId.FromGuid(request.Id);
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment is null)
            {
                return ApiResponse<AppointmentDto>.ErrorResult("Appointment not found");
            }

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

            var updateResult = appointment.UpdateDetails(
                patientId,
                doctorId,
                request.ScheduledAt,
                request.DurationMinutes,
                request.Reason,
                request.Notes,
                status);

            if (!updateResult.IsSuccess)
            {
                return ApiResponse<AppointmentDto>.ErrorResult(updateResult.Error.Message);
            }

            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            await _appointmentRepository.SaveChangesAsync(cancellationToken);

            return ApiResponse<AppointmentDto>.SuccessResult(_mapper.Map<AppointmentDto>(appointment), "Appointment updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<AppointmentDto>.ErrorResult("An error occurred while updating the appointment", new List<string> { ex.Message });
        }
    }
}
