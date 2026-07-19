using AutoMapper;
using MediatR;
using SmartCare.Application.Common.DTOs;
using SmartCare.Domain.Repositories;
using SmartCare.Domain.Services;
using SmartCare.Domain.ValueObjects;

namespace SmartCare.Application.MedicalRecords.Commands;

public record UpdateMedicalRecordCommand(
    Guid Id,
    Guid PatientId,
    string Diagnosis,
    string Treatment,
    string Notes,
    DateTime RecordDate,
    string DoctorId) : IRequest<ApiResponse<MedicalRecordDto>>;

public sealed class UpdateMedicalRecordCommandHandler : IRequestHandler<UpdateMedicalRecordCommand, ApiResponse<MedicalRecordDto>>
{
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IPatientDomainService _patientDomainService;
    private readonly IMapper _mapper;

    public UpdateMedicalRecordCommandHandler(
        IMedicalRecordRepository medicalRecordRepository,
        IPatientRepository patientRepository,
        IPatientDomainService patientDomainService,
        IMapper mapper)
    {
        _medicalRecordRepository = medicalRecordRepository;
        _patientRepository = patientRepository;
        _patientDomainService = patientDomainService;
        _mapper = mapper;
    }

    public async Task<ApiResponse<MedicalRecordDto>> Handle(UpdateMedicalRecordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var recordId = MedicalRecordId.FromGuid(request.Id);
            var medicalRecord = await _medicalRecordRepository.GetByIdAsync(recordId, cancellationToken);
            if (medicalRecord is null)
            {
                return ApiResponse<MedicalRecordDto>.ErrorResult("Medical record not found");
            }

            var patientId = PatientId.FromGuid(request.PatientId);
            if (await _patientRepository.GetByIdAsync(patientId, cancellationToken) is null)
            {
                return ApiResponse<MedicalRecordDto>.ErrorResult("Patient not found");
            }

            var accessResult = await _patientDomainService.ValidateMedicalRecordAccessAsync(patientId, request.DoctorId, cancellationToken);
            if (!accessResult.IsSuccess)
            {
                return ApiResponse<MedicalRecordDto>.ErrorResult("Access denied");
            }

            medicalRecord.UpdateDetails(request.Diagnosis, request.Treatment, request.Notes, request.DoctorId);
            medicalRecord.UpdateRecordDate(request.RecordDate);

            await _medicalRecordRepository.UpdateAsync(medicalRecord, cancellationToken);
            await _medicalRecordRepository.SaveChangesAsync(cancellationToken);

            return ApiResponse<MedicalRecordDto>.SuccessResult(_mapper.Map<MedicalRecordDto>(medicalRecord), "Medical record updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<MedicalRecordDto>.ErrorResult("An error occurred while updating the medical record", new List<string> { ex.Message });
        }
    }
}
