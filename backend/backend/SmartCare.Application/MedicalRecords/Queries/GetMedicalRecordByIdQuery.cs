using AutoMapper;
using MediatR;
using SmartCare.Application.Common.DTOs;
using SmartCare.Domain.Repositories;
using SmartCare.Domain.ValueObjects;

namespace SmartCare.Application.MedicalRecords.Queries;

public record GetMedicalRecordByIdQuery(Guid Id) : IRequest<ApiResponse<MedicalRecordDto>>;

public sealed class GetMedicalRecordByIdQueryHandler : IRequestHandler<GetMedicalRecordByIdQuery, ApiResponse<MedicalRecordDto>>
{
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly IMapper _mapper;

    public GetMedicalRecordByIdQueryHandler(IMedicalRecordRepository medicalRecordRepository, IMapper mapper)
    {
        _medicalRecordRepository = medicalRecordRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<MedicalRecordDto>> Handle(GetMedicalRecordByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var record = await _medicalRecordRepository.GetByIdAsync(MedicalRecordId.FromGuid(request.Id), cancellationToken);
            if (record is null)
            {
                return ApiResponse<MedicalRecordDto>.ErrorResult("Medical record not found");
            }

            return ApiResponse<MedicalRecordDto>.SuccessResult(_mapper.Map<MedicalRecordDto>(record));
        }
        catch (Exception ex)
        {
            return ApiResponse<MedicalRecordDto>.ErrorResult("An error occurred while retrieving the medical record", new List<string> { ex.Message });
        }
    }
}
