using MediatR;
using SmartCare.Application.Common.DTOs;
using SmartCare.Domain.Repositories;
using SmartCare.Domain.ValueObjects;

namespace SmartCare.Application.Doctors.Commands;

public record DeleteDoctorCommand(Guid Id) : IRequest<ApiResponse>;

public sealed class DeleteDoctorCommandHandler : IRequestHandler<DeleteDoctorCommand, ApiResponse>
{
    private readonly IDoctorRepository _doctorRepository;

    public DeleteDoctorCommandHandler(IDoctorRepository doctorRepository)
    {
        _doctorRepository = doctorRepository;
    }

    public async Task<ApiResponse> Handle(DeleteDoctorCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var doctorId = DoctorId.FromGuid(request.Id);
            var doctor = await _doctorRepository.GetByIdAsync(doctorId, cancellationToken);
            if (doctor is null)
            {
                return ApiResponse.ErrorResult("Doctor not found");
            }

            await _doctorRepository.DeleteAsync(doctorId, cancellationToken);
            await _doctorRepository.SaveChangesAsync(cancellationToken);

            return ApiResponse.CreateSuccess("Doctor deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse.ErrorResult("An error occurred while deleting the doctor", new List<string> { ex.Message });
        }
    }
}
