using AutoMapper;
using MediatR;
using SmartCare.Application.Common.DTOs;
using SmartCare.Domain.Repositories;
using SmartCare.Domain.ValueObjects;

namespace SmartCare.Application.Doctors.Commands;

public record CreateDoctorCommand(
    string FirstName,
    string LastName,
    string Email,
    string Specialty,
    string PhoneNumber,
    string LicenseNumber) : IRequest<ApiResponse<DoctorDto>>;

public sealed class CreateDoctorCommandHandler : IRequestHandler<CreateDoctorCommand, ApiResponse<DoctorDto>>
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IMapper _mapper;

    public CreateDoctorCommandHandler(IDoctorRepository doctorRepository, IMapper mapper)
    {
        _doctorRepository = doctorRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<DoctorDto>> Handle(CreateDoctorCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var emailResult = Email.Create(request.Email);
            if (!emailResult.IsSuccess)
            {
                return ApiResponse<DoctorDto>.ErrorResult("Invalid email format");
            }

            if (await _doctorRepository.EmailExistsAsync(emailResult.Value, cancellationToken))
            {
                return ApiResponse<DoctorDto>.ErrorResult("A doctor with this email already exists");
            }

            if (await _doctorRepository.LicenseNumberExistsAsync(request.LicenseNumber, cancellationToken))
            {
                return ApiResponse<DoctorDto>.ErrorResult("A doctor with this license number already exists");
            }

            var nameResult = FullName.Create(request.FirstName, request.LastName);
            if (!nameResult.IsSuccess)
            {
                return ApiResponse<DoctorDto>.ErrorResult("Invalid name provided");
            }

            var doctorId = DoctorId.Create();
            var doctorResult = Domain.Entities.Doctor.Create(
                doctorId,
                nameResult.Value,
                emailResult.Value,
                request.Specialty,
                request.PhoneNumber,
                request.LicenseNumber);

            if (!doctorResult.IsSuccess)
            {
                return ApiResponse<DoctorDto>.ErrorResult(doctorResult.Error.Message);
            }

            var doctor = doctorResult.Value;
            await _doctorRepository.AddAsync(doctor, cancellationToken);
            await _doctorRepository.SaveChangesAsync(cancellationToken);

            return ApiResponse<DoctorDto>.SuccessResult(_mapper.Map<DoctorDto>(doctor), "Doctor created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<DoctorDto>.ErrorResult("An error occurred while creating the doctor", new List<string> { ex.Message });
        }
    }
}
