using MediatR;
using SmartCare.Application.Common.DTOs;
using SmartCare.Domain.Repositories;
using SmartCare.Domain.Services;
using AutoMapper;

namespace SmartCare.Application.Patients.Commands;

public record DeletePatientCommand(Guid Id) : IRequest<ApiResponse>;

public class DeletePatientCommandHandler : IRequestHandler<DeletePatientCommand, ApiResponse>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IPatientDomainService _patientDomainService;

    public DeletePatientCommandHandler(IPatientRepository patientRepository, IPatientDomainService patientDomainService)
    {
        _patientRepository = patientRepository;
        _patientDomainService = patientDomainService;
    }

    public async Task<ApiResponse> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var patientId = Domain.ValueObjects.PatientId.FromGuid(request.Id);
            var patient = await _patientRepository.GetByIdAsync(patientId, cancellationToken);

            if (patient == null)
            {
                return ApiResponse.ErrorResult("Patient not found");
            }

            // Check if patient can be deleted
            var canDeleteResult = await _patientDomainService.CanPatientBeDeletedAsync(patientId, cancellationToken);
            if (!canDeleteResult.IsSuccess || !canDeleteResult.Value)
            {
                return ApiResponse.ErrorResult("Patient cannot be deleted due to existing medical records");
            }

            await _patientRepository.DeleteAsync(patientId, cancellationToken);
            await _patientRepository.SaveChangesAsync(cancellationToken);

            return ApiResponse.CreateSuccess("Patient deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse.ErrorResult("An error occurred while deleting the patient", new List<string> { ex.Message });
        }
    }
}
