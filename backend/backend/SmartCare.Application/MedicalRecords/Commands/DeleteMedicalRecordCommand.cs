using MediatR;
using SmartCare.Application.Common.DTOs;
using SmartCare.Domain.Repositories;
using SmartCare.Domain.ValueObjects;

namespace SmartCare.Application.MedicalRecords.Commands;

public record DeleteMedicalRecordCommand(Guid Id) : IRequest<ApiResponse>;

public sealed class DeleteMedicalRecordCommandHandler : IRequestHandler<DeleteMedicalRecordCommand, ApiResponse>
{
    private readonly IMedicalRecordRepository _medicalRecordRepository;

    public DeleteMedicalRecordCommandHandler(IMedicalRecordRepository medicalRecordRepository)
    {
        _medicalRecordRepository = medicalRecordRepository;
    }

    public async Task<ApiResponse> Handle(DeleteMedicalRecordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var recordId = MedicalRecordId.FromGuid(request.Id);
            if (await _medicalRecordRepository.GetByIdAsync(recordId, cancellationToken) is null)
            {
                return ApiResponse.ErrorResult("Medical record not found");
            }

            await _medicalRecordRepository.DeleteAsync(recordId, cancellationToken);
            await _medicalRecordRepository.SaveChangesAsync(cancellationToken);

            return ApiResponse.CreateSuccess("Medical record deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse.ErrorResult("An error occurred while deleting the medical record", new List<string> { ex.Message });
        }
    }
}
