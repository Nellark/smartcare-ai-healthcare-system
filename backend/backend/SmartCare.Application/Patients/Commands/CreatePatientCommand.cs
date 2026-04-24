using MediatR;
using SmartCare.Application.Common.DTOs;
using SmartCare.Domain.Repositories;
using SmartCare.Domain.Services;
using SmartCare.Domain.ValueObjects;
using AutoMapper;

namespace SmartCare.Application.Patients.Commands;

public record CreatePatientCommand(
    string FirstName,
    string LastName,
    string Email,
    DateTime DateOfBirth,
    string PhoneNumber,
    string Address) : IRequest<ApiResponse<PatientDto>>;

public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, ApiResponse<PatientDto>>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper _mapper;

    public CreatePatientCommandHandler(IPatientRepository patientRepository, IMapper mapper)
    {
        _patientRepository = patientRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PatientDto>> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if email already exists
            var emailResult = Domain.ValueObjects.Email.Create(request.Email);
            if (!emailResult.IsSuccess)
            {
                return ApiResponse<PatientDto>.ErrorResult("Invalid email format");
            }

            var existingPatients = await _patientRepository.GetByEmailAsync(emailResult.Value, cancellationToken);
            if (existingPatients.Any())
            {
                return ApiResponse<PatientDto>.ErrorResult("A patient with this email already exists");
            }

            // Create name value object
            var nameResult = Domain.ValueObjects.FullName.Create(request.FirstName, request.LastName);
            if (!nameResult.IsSuccess)
            {
                return ApiResponse<PatientDto>.ErrorResult("Invalid name provided");
            }

            // Create patient
            var patientId = Domain.ValueObjects.PatientId.Create();
            var patientResult = Domain.Entities.Patient.Create(
                patientId,
                nameResult.Value,
                emailResult.Value,
                request.DateOfBirth,
                request.PhoneNumber,
                request.Address);

            if (!patientResult.IsSuccess)
            {
                return ApiResponse<PatientDto>.ErrorResult(patientResult.Error.Message);
            }

            var patient = patientResult.Value;
            await _patientRepository.AddAsync(patient, cancellationToken);
            await _patientRepository.SaveChangesAsync(cancellationToken);

            var patientDto = _mapper.Map<PatientDto>(patient);
            return ApiResponse<PatientDto>.SuccessResult(patientDto, "Patient created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<PatientDto>.ErrorResult("An error occurred while creating the patient", new List<string> { ex.Message });
        }
    }
}
