using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCare.Application.Appointments.Commands;
using SmartCare.Application.Appointments.Queries;
using SmartCare.Application.Common.DTOs;

namespace SmartCare.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AppointmentsController> _logger;

    public AppointmentsController(IMediator mediator, ILogger<AppointmentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Policy = "ManageAppointments")]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> CreateAppointment([FromBody] CreateAppointmentCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return result.Success
                ? CreatedAtAction(nameof(GetAppointmentById), new { id = result.Data?.Id }, result)
                : BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating appointment");
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<AppointmentDto>.ErrorResult("An unexpected error occurred"));
        }
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "ViewAppointments")]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> GetAppointmentById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetAppointmentByIdQuery(id));
            return result.Success ? Ok(result) : NotFound(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving appointment {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<AppointmentDto>.ErrorResult("An unexpected error occurred"));
        }
    }

    [HttpGet]
    [Authorize(Policy = "ViewAppointments")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AppointmentDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AppointmentDto>>>> GetAppointments(
        [FromQuery] Guid? patientId,
        [FromQuery] Guid? doctorId)
    {
        try
        {
            if (patientId.HasValue)
            {
                return Ok(await _mediator.Send(new GetAppointmentsByPatientQuery(patientId.Value)));
            }

            if (doctorId.HasValue)
            {
                return Ok(await _mediator.Send(new GetAppointmentsByDoctorQuery(doctorId.Value)));
            }

            return Ok(await _mediator.Send(new GetAllAppointmentsQuery()));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving appointments");
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<IReadOnlyList<AppointmentDto>>.ErrorResult("An unexpected error occurred"));
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "ManageAppointments")]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> UpdateAppointment(Guid id, [FromBody] UpdateAppointmentCommand command)
    {
        try
        {
            if (id != command.Id)
            {
                return BadRequest(ApiResponse<AppointmentDto>.ErrorResult("ID mismatch"));
            }

            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating appointment {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<AppointmentDto>.ErrorResult("An unexpected error occurred"));
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "DeleteAppointments")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> DeleteAppointment(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteAppointmentCommand(id));
            return result.Success ? Ok(result) : BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting appointment {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.ErrorResult("An unexpected error occurred"));
        }
    }
}
