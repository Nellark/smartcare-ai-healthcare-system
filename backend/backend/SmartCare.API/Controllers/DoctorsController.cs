using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCare.Application.Common.DTOs;
using SmartCare.Application.Doctors.Commands;
using SmartCare.Application.Doctors.Queries;

namespace SmartCare.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DoctorsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<DoctorsController> _logger;

    public DoctorsController(IMediator mediator, ILogger<DoctorsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Policy = "ManageDoctors")]
    [ProducesResponseType(typeof(ApiResponse<DoctorDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<DoctorDto>>> CreateDoctor([FromBody] CreateDoctorCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return result.Success
                ? CreatedAtAction(nameof(GetDoctorById), new { id = result.Data?.Id }, result)
                : BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating doctor");
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<DoctorDto>.ErrorResult("An unexpected error occurred"));
        }
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "ViewDoctors")]
    [ProducesResponseType(typeof(ApiResponse<DoctorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<DoctorDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<DoctorDto>>> GetDoctorById(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new GetDoctorByIdQuery(id));
            return result.Success ? Ok(result) : NotFound(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctor {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<DoctorDto>.ErrorResult("An unexpected error occurred"));
        }
    }

    [HttpGet]
    [Authorize(Policy = "ViewDoctors")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<DoctorDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DoctorDto>>>> GetAllDoctors()
    {
        try
        {
            var result = await _mediator.Send(new GetAllDoctorsQuery());
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctors");
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<IReadOnlyList<DoctorDto>>.ErrorResult("An unexpected error occurred"));
        }
    }

    [HttpGet("search")]
    [Authorize(Policy = "ViewDoctors")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<DoctorDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DoctorDto>>>> SearchDoctors([FromQuery] string specialty)
    {
        try
        {
            var result = await _mediator.Send(new GetDoctorsBySpecialtyQuery(specialty));
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching doctors");
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<IReadOnlyList<DoctorDto>>.ErrorResult("An unexpected error occurred"));
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "ManageDoctors")]
    [ProducesResponseType(typeof(ApiResponse<DoctorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<DoctorDto>>> UpdateDoctor(Guid id, [FromBody] UpdateDoctorCommand command)
    {
        try
        {
            if (id != command.Id)
            {
                return BadRequest(ApiResponse<DoctorDto>.ErrorResult("ID mismatch"));
            }

            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating doctor {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<DoctorDto>.ErrorResult("An unexpected error occurred"));
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "DeleteDoctors")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> DeleteDoctor(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new DeleteDoctorCommand(id));
            return result.Success ? Ok(result) : BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting doctor {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.ErrorResult("An unexpected error occurred"));
        }
    }
}
