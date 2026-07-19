using AutoMapper;
using MediatR;
using SmartCare.Application.Common.DTOs;
using SmartCare.Domain.Repositories;
using SmartCare.Domain.ValueObjects;

namespace SmartCare.Application.MedicalRecords.Queries;

public record GetMedicalRecordsByPatientQuery(Guid PatientId) : IRequest<ApiResponse<IReadOnlyList<MedicalRecordDto>>>;

public sealed class GetMedicalRecordsByPatientQueryHandler : IRequestHandler<GetMedicalRecordsByPatientQuery, ApiResponse<IReadOnlyList<MedicalRecordDto>>>
{
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly IMapper _mapper;

    public GetMedicalRecordsByPatientQueryHandler(IMedicalRecordRepository medicalRecordRepository, IMapper mapper)
    {
        _medicalRecordRepository = medicalRecordRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IReadOnlyList<MedicalRecordDto>>> Handle(GetMedicalRecordsByPatientQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var records = await _medicalRecordRepository.GetByPatientIdAsync(PatientId.FromGuid(request.PatientId), cancellationToken);
            return ApiResponse<IReadOnlyList<MedicalRecordDto>>.SuccessResult(_mapper.Map<IReadOnlyList<MedicalRecordDto>>(records));
        }
        catch (Exception ex)
        {
            return ApiResponse<IReadOnlyList<MedicalRecordDto>>.ErrorResult("An error occurred while retrieving medical records", new List<string> { ex.Message });
        }
    }
}
