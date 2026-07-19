using MediatR;
using SmartCare.Application.Common.DTOs;
using SmartCare.Domain.Repositories;
using SmartCare.Domain.ValueObjects;

namespace SmartCare.Application.Appointments.Commands;

public record DeleteAppointmentCommand(Guid Id) : IRequest<ApiResponse>;

public sealed class DeleteAppointmentCommandHandler : IRequestHandler<DeleteAppointmentCommand, ApiResponse>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public DeleteAppointmentCommandHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<ApiResponse> Handle(DeleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var appointmentId = AppointmentId.FromGuid(request.Id);
            if (await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken) is null)
            {
                return ApiResponse.ErrorResult("Appointment not found");
            }

            await _appointmentRepository.DeleteAsync(appointmentId, cancellationToken);
            await _appointmentRepository.SaveChangesAsync(cancellationToken);

            return ApiResponse.CreateSuccess("Appointment deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse.ErrorResult("An error occurred while deleting the appointment", new List<string> { ex.Message });
        }
    }
}
