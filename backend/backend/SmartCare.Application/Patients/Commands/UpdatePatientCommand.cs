using MediatR;
using SmartCare.Application.Common.DTOs;
using SmartCare.Domain.Repositories;
using SmartCare.Domain.ValueObjects;
using AutoMapper;

namespace SmartCare.Application.Patients.Commands;

public record UpdatePatientCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Address) : IRequest<ApiResponse<PatientDto>>;

public class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand, ApiResponse<PatientDto>>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper _mapper;

    public UpdatePatientCommandHandler(IPatientRepository patientRepository, IMapper mapper)
    {
        _patientRepository = patientRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PatientDto>> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var patientId = Domain.ValueObjects.PatientId.FromGuid(request.Id);
            var patient = await _patientRepository.GetByIdAsync(patientId, cancellationToken);

            if (patient == null)
            {
                return ApiResponse<PatientDto>.ErrorResult("Patient not found");
            }

            // Create value objects
            var nameResult = Domain.ValueObjects.FullName.Create(request.FirstName, request.LastName);
            if (!nameResult.IsSuccess)
            {
                return ApiResponse<PatientDto>.ErrorResult("Invalid name provided");
            }

            var emailResult = Domain.ValueObjects.Email.Create(request.Email);
            if (!emailResult.IsSuccess)
            {
                return ApiResponse<PatientDto>.ErrorResult("Invalid email format");
            }

            // Check if email is being changed and if it's already taken
            var existingPatients = await _patientRepository.GetByEmailAsync(emailResult.Value, cancellationToken);
            if (existingPatients.Any(p => p.Id != patientId))
            {
                return ApiResponse<PatientDto>.ErrorResult("A patient with this email already exists");
            }

            // Update patient
            var updateResult = patient.UpdatePersonalInformation(
                nameResult.Value,
                emailResult.Value,
                request.PhoneNumber,
                request.Address);

            if (!updateResult.IsSuccess)
            {
                return ApiResponse<PatientDto>.ErrorResult(updateResult.Error.Message);
            }

            await _patientRepository.UpdateAsync(patient, cancellationToken);
            await _patientRepository.SaveChangesAsync(cancellationToken);

            var patientDto = _mapper.Map<PatientDto>(patient);
            return ApiResponse<PatientDto>.SuccessResult(patientDto, "Patient updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<PatientDto>.ErrorResult("An error occurred while updating the patient", new List<string> { ex.Message });
        }
    }
}
