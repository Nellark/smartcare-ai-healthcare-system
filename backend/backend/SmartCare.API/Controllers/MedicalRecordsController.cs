using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCare.Application.Common.DTOs;
using SmartCare.Application.MedicalRecords.Commands;
using SmartCare.Application.MedicalRecords.Queries;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace SmartCare.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MedicalRecordsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<MedicalRecordsController> _logger;
    private readonly IWebHostEnvironment _environment;

    public MedicalRecordsController(IMediator mediator, ILogger<MedicalRecordsController> logger, IWebHostEnvironment environment)
    {
        _mediator = mediator;
        _logger = logger;
        _environment = environment;
    }

    [HttpPost]
    [Authorize(Policy = "ManageMedicalRecords")]
    [ProducesResponseType(typeof(ApiResponse<MedicalRecordDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<MedicalRecordDto>>> CreateMedicalRecord([FromBody] CreateMedicalRecordCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return result.Success
                ? CreatedAtAction(nameof(GetMedicalRecordById), new { id = result.Data?.Id }, result)
                : BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating medical record");
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<MedicalRecordDto>.ErrorResult("An unexpected error occurred"));
        }
    }

    [HttpPost("upload")]
    [Authorize(Policy = "ManageMedicalRecords")]
    [ProducesResponseType(typeof(ApiResponse<MedicalRecordDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<MedicalRecordDto>>> UploadMedicalRecord([FromForm] UploadMedicalRecordRequest request)
    {
        try
        {
            string? attachmentUrl = null;
            if (request.File != null && request.File.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + request.File.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(fileStream);
                }

                attachmentUrl = $"/uploads/{uniqueFileName}";
            }

            var command = new CreateMedicalRecordCommand(
                request.PatientId,
                request.Diagnosis ?? "",
                request.Treatment ?? "",
                request.Notes ?? "",
                request.RecordDate,
                request.DoctorId,
                request.RecordType ?? "GENERAL",
                request.Title ?? "Medical Record",
                request.Provider ?? "",
                request.Status ?? "COMPLETED",
                attachmentUrl
            );

            var result = await _mediator.Send(command);
            return result.Success
                ? CreatedAtAction(nameof(GetMedicalRecordById), new { id = result.Data?.Id }, result)
                : BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading medical record");
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<MedicalRecordDto>.ErrorResult("An unexpected error occurred"));
        }
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "ViewMedicalRecords")]
    [ProducesResponseType(typeof(ApiResponse<MedicalRecordDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<MedicalRecordDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<MedicalRecordDto>>> GetMedicalRecordById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetMedicalRecordByIdQuery(id));
            return result.Success ? Ok(result) : NotFound(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving medical record {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<MedicalRecordDto>.ErrorResult("An unexpected error occurred"));
        }
    }

    [HttpGet]
    [Authorize(Policy = "ViewMedicalRecords")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<MedicalRecordDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<MedicalRecordDto>>>> GetMedicalRecords([FromQuery] Guid? patientId)
    {
        try
        {
            if (patientId.HasValue)
            {
                return Ok(await _mediator.Send(new GetMedicalRecordsByPatientQuery(patientId.Value)));
            }

            return Ok(await _mediator.Send(new GetAllMedicalRecordsQuery()));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving medical records");
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<IReadOnlyList<MedicalRecordDto>>.ErrorResult("An unexpected error occurred"));
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "ManageMedicalRecords")]
    [ProducesResponseType(typeof(ApiResponse<MedicalRecordDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<MedicalRecordDto>>> UpdateMedicalRecord(Guid id, [FromBody] UpdateMedicalRecordCommand command)
    {
        try
        {
            if (id != command.Id)
            {
                return BadRequest(ApiResponse<MedicalRecordDto>.ErrorResult("ID mismatch"));
            }

            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating medical record {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<MedicalRecordDto>.ErrorResult("An unexpected error occurred"));
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "DeleteMedicalRecords")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> DeleteMedicalRecord(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteMedicalRecordCommand(id));
            return result.Success ? Ok(result) : BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting medical record {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.ErrorResult("An unexpected error occurred"));
        }
    }
}

public class UploadMedicalRecordRequest
{
    public Guid PatientId { get; set; }
    public string? Diagnosis { get; set; }
    public string? Treatment { get; set; }
    public string? Notes { get; set; }
    public DateTime RecordDate { get; set; }
    public string DoctorId { get; set; } = string.Empty;
    public string? RecordType { get; set; }
    public string? Title { get; set; }
    public string? Provider { get; set; }
    public string? Status { get; set; }
    public IFormFile? File { get; set; }
}
