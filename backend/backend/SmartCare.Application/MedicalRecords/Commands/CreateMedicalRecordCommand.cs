using AutoMapper;
using MediatR;
using SmartCare.Application.Common.DTOs;
using SmartCare.Domain.Repositories;
using SmartCare.Domain.Services;
using SmartCare.Domain.ValueObjects;

namespace SmartCare.Application.MedicalRecords.Commands;

public record CreateMedicalRecordCommand(
    Guid PatientId,
    string Diagnosis,
    string Treatment,
    string Notes,
    DateTime RecordDate,
    string DoctorId) : IRequest<ApiResponse<MedicalRecordDto>>;

public sealed class CreateMedicalRecordCommandHandler : IRequestHandler<CreateMedicalRecordCommand, ApiResponse<MedicalRecordDto>>
{
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IPatientDomainService _patientDomainService;
    private readonly IMapper _mapper;

    public CreateMedicalRecordCommandHandler(
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

    public async Task<ApiResponse<MedicalRecordDto>> Handle(CreateMedicalRecordCommand request, CancellationToken cancellationToken)
    {
        try
        {
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

            var medicalRecord = Domain.Entities.MedicalRecord.Create(
                MedicalRecordId.Create(),
                patientId,
                request.Diagnosis,
                request.Treatment,
                request.Notes,
                request.RecordDate,
                request.DoctorId);

            await _medicalRecordRepository.AddAsync(medicalRecord, cancellationToken);
            await _medicalRecordRepository.SaveChangesAsync(cancellationToken);

            return ApiResponse<MedicalRecordDto>.SuccessResult(_mapper.Map<MedicalRecordDto>(medicalRecord), "Medical record created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<MedicalRecordDto>.ErrorResult("An error occurred while creating the medical record", new List<string> { ex.Message });
        }
    }
}
