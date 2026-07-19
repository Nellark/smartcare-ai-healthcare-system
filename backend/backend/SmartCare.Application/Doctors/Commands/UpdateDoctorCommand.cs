using AutoMapper;
using MediatR;
using SmartCare.Application.Common.DTOs;
using SmartCare.Domain.Repositories;
using SmartCare.Domain.ValueObjects;

namespace SmartCare.Application.Doctors.Commands;

public record UpdateDoctorCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Specialty,
    string PhoneNumber,
    string LicenseNumber) : IRequest<ApiResponse<DoctorDto>>;

public sealed class UpdateDoctorCommandHandler : IRequestHandler<UpdateDoctorCommand, ApiResponse<DoctorDto>>
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IMapper _mapper;

    public UpdateDoctorCommandHandler(IDoctorRepository doctorRepository, IMapper mapper)
    {
        _doctorRepository = doctorRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<DoctorDto>> Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var doctorId = DoctorId.FromGuid(request.Id);
            var doctor = await _doctorRepository.GetByIdAsync(doctorId, cancellationToken);
            if (doctor is null)
            {
                return ApiResponse<DoctorDto>.ErrorResult("Doctor not found");
            }

            var nameResult = FullName.Create(request.FirstName, request.LastName);
            if (!nameResult.IsSuccess)
            {
                return ApiResponse<DoctorDto>.ErrorResult("Invalid name provided");
            }

            var emailResult = Email.Create(request.Email);
            if (!emailResult.IsSuccess)
            {
                return ApiResponse<DoctorDto>.ErrorResult("Invalid email format");
            }

            var existingDoctors = await _doctorRepository.GetByEmailAsync(emailResult.Value, cancellationToken);
            if (existingDoctors.Any(x => x.Id != doctorId))
            {
                return ApiResponse<DoctorDto>.ErrorResult("A doctor with this email already exists");
            }

            var existingByLicense = await _doctorRepository.GetAllAsync(cancellationToken);
            if (existingByLicense.Any(x => x.Id != doctorId && x.LicenseNumber == request.LicenseNumber))
            {
                return ApiResponse<DoctorDto>.ErrorResult("A doctor with this license number already exists");
            }

            var updateResult = doctor.UpdateDetails(
                nameResult.Value,
                emailResult.Value,
                request.Specialty,
                request.PhoneNumber,
                request.LicenseNumber);

            if (!updateResult.IsSuccess)
            {
                return ApiResponse<DoctorDto>.ErrorResult(updateResult.Error.Message);
            }

            await _doctorRepository.UpdateAsync(doctor, cancellationToken);
            await _doctorRepository.SaveChangesAsync(cancellationToken);

            return ApiResponse<DoctorDto>.SuccessResult(_mapper.Map<DoctorDto>(doctor), "Doctor updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<DoctorDto>.ErrorResult("An error occurred while updating the doctor", new List<string> { ex.Message });
        }
    }
}
