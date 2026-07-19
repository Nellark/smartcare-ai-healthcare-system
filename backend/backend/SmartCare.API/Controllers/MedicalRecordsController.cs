using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCare.Application.Common.DTOs;
using SmartCare.Application.MedicalRecords.Commands;
using SmartCare.Application.MedicalRecords.Queries;

namespace SmartCare.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MedicalRecordsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<MedicalRecordsController> _logger;

    public MedicalRecordsController(IMediator mediator, ILogger<MedicalRecordsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
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
