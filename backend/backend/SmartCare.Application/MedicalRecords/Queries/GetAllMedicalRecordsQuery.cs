using AutoMapper;
using MediatR;
using SmartCare.Application.Common.DTOs;
using SmartCare.Domain.Repositories;

namespace SmartCare.Application.MedicalRecords.Queries;

public record GetAllMedicalRecordsQuery : IRequest<ApiResponse<IReadOnlyList<MedicalRecordDto>>>;

public sealed class GetAllMedicalRecordsQueryHandler : IRequestHandler<GetAllMedicalRecordsQuery, ApiResponse<IReadOnlyList<MedicalRecordDto>>>
{
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly IMapper _mapper;

    public GetAllMedicalRecordsQueryHandler(IMedicalRecordRepository medicalRecordRepository, IMapper mapper)
    {
        _medicalRecordRepository = medicalRecordRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IReadOnlyList<MedicalRecordDto>>> Handle(GetAllMedicalRecordsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var records = await _medicalRecordRepository.GetAllAsync(cancellationToken);
            return ApiResponse<IReadOnlyList<MedicalRecordDto>>.SuccessResult(_mapper.Map<IReadOnlyList<MedicalRecordDto>>(records));
        }
        catch (Exception ex)
        {
            return ApiResponse<IReadOnlyList<MedicalRecordDto>>.ErrorResult("An error occurred while retrieving medical records", new List<string> { ex.Message });
        }
    }
}
