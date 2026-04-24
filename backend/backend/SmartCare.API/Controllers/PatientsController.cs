using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartCare.Application.Common.DTOs;
using SmartCare.Application.Patients.Commands;
using SmartCare.Application.Patients.Queries;

namespace SmartCare.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PatientsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PatientsController> _logger;

    public PatientsController(IMediator mediator, ILogger<PatientsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new patient
    /// </summary>
    /// <param name="command">Patient creation data</param>
    /// <returns>Created patient details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PatientDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<PatientDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PatientDto>>> CreatePatient([FromBody] CreatePatientCommand command)
    {
        try
        {
            _logger.LogInformation("Creating new patient with email: {Email}", command.Email);
            
            var result = await _mediator.Send(command);
            
            if (result.Success)
            {
                _logger.LogInformation("Patient created successfully with ID: {Id}", result.Data?.Id);
                return CreatedAtAction(nameof(GetPatientById), new { id = result.Data?.Id }, result);
            }
            
            _logger.LogWarning("Failed to create patient: {Message}", result.Message);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating patient");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                ApiResponse<PatientDto>.ErrorResult("An unexpected error occurred"));
        }
    }

    /// <summary>
    /// Gets a patient by ID
    /// </summary>
    /// <param name="id">Patient ID</param>
    /// <returns>Patient details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PatientDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PatientDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PatientDto>>> GetPatientById(Guid id)
    {
        try
        {
            _logger.LogInformation("Retrieving patient with ID: {Id}", id);
            
            var query = new GetPatientByIdQuery(id);
            var result = await _mediator.Send(query);
            
            if (result.Success)
            {
                _logger.LogInformation("Patient retrieved successfully: {Id}", id);
                return Ok(result);
            }
            
            _logger.LogWarning("Patient not found: {Id}", id);
            return NotFound(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving patient {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                ApiResponse<PatientDto>.ErrorResult("An unexpected error occurred"));
        }
    }

    /// <summary>
    /// Gets all patients
    /// </summary>
    /// <returns>List of all patients</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PatientDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<PatientDto>>>> GetAllPatients()
    {
        try
        {
            _logger.LogInformation("Retrieving all patients");
            
            var query = new GetAllPatientsQuery();
            var result = await _mediator.Send(query);
            
            _logger.LogInformation("Retrieved {Count} patients", result.Data?.Count ?? 0);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all patients");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                ApiResponse<IReadOnlyList<PatientDto>>.ErrorResult("An unexpected error occurred"));
        }
    }

    /// <summary>
    /// Searches patients by name
    /// </summary>
    /// <param name="firstName">First name</param>
    /// <param name="lastName">Last name</param>
    /// <returns>List of matching patients</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PatientDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<PatientDto>>>> SearchPatients(
        [FromQuery] string firstName,
        [FromQuery] string lastName)
    {
        try
        {
            _logger.LogInformation("Searching patients by name: {FirstName} {LastName}", firstName, lastName);
            
            var query = new GetPatientsByNameQuery(firstName, lastName);
            var result = await _mediator.Send(query);
            
            _logger.LogInformation("Found {Count} patients matching search criteria", result.Data?.Count ?? 0);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching patients");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                ApiResponse<IReadOnlyList<PatientDto>>.ErrorResult("An unexpected error occurred"));
        }
    }

    /// <summary>
    /// Updates an existing patient
    /// </summary>
    /// <param name="id">Patient ID</param>
    /// <param name="command">Patient update data</param>
    /// <returns>Updated patient details</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PatientDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PatientDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<PatientDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PatientDto>>> UpdatePatient(
        Guid id, 
        [FromBody] UpdatePatientCommand command)
    {
        try
        {
            if (id != command.Id)
            {
                return BadRequest(ApiResponse<PatientDto>.ErrorResult("ID mismatch"));
            }

            _logger.LogInformation("Updating patient with ID: {Id}", id);
            
            var result = await _mediator.Send(command);
            
            if (result.Success)
            {
                _logger.LogInformation("Patient updated successfully: {Id}", id);
                return Ok(result);
            }
            
            if (result.Message?.Contains("not found") ?? false)
            {
                _logger.LogWarning("Patient not found for update: {Id}", id);
                return NotFound(result);
            }
            
            _logger.LogWarning("Failed to update patient {Id}: {Message}", id, result.Message);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating patient {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                ApiResponse<PatientDto>.ErrorResult("An unexpected error occurred"));
        }
    }

    /// <summary>
    /// Deletes a patient
    /// </summary>
    /// <param name="id">Patient ID</param>
    /// <returns>Deletion result</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> DeletePatient(Guid id)
    {
        try
        {
            _logger.LogInformation("Deleting patient with ID: {Id}", id);
            
            var command = new DeletePatientCommand(id);
            var result = await _mediator.Send(command);
            
            if (result.Success)
            {
                _logger.LogInformation("Patient deleted successfully: {Id}", id);
                return Ok(result);
            }
            
            if (result.Message?.Contains("not found") ?? false)
            {
                _logger.LogWarning("Patient not found for deletion: {Id}", id);
                return NotFound(result);
            }
            
            _logger.LogWarning("Failed to delete patient {Id}: {Message}", id, result.Message);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting patient {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                ApiResponse.ErrorResult("An unexpected error occurred"));
        }
    }

    /// <summary>
    /// Adds a medical record to a patient
    /// </summary>
    /// <param name="patientId">Patient ID</param>
    /// <param name="command">Medical record data</param>
    /// <returns>Created medical record details</returns>
    [HttpPost("{patientId:guid}/medical-records")]
    [ProducesResponseType(typeof(ApiResponse<MedicalRecordDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<MedicalRecordDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<MedicalRecordDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<MedicalRecordDto>>> AddMedicalRecord(
        Guid patientId,
        [FromBody] AddMedicalRecordCommand command)
    {
        try
        {
            if (patientId != command.PatientId)
            {
                return BadRequest(ApiResponse<MedicalRecordDto>.ErrorResult("Patient ID mismatch"));
            }

            _logger.LogInformation("Adding medical record to patient {PatientId}", patientId);
            
            var result = await _mediator.Send(command);
            
            if (result.Success)
            {
                _logger.LogInformation("Medical record added successfully to patient {PatientId}", patientId);
                return CreatedAtAction(nameof(GetPatientById), new { id = patientId }, result);
            }
            
            if (result.Message?.Contains("not found") ?? false)
            {
                _logger.LogWarning("Patient not found for medical record addition: {PatientId}", patientId);
                return NotFound(result);
            }
            
            _logger.LogWarning("Failed to add medical record to patient {PatientId}: {Message}", patientId, result.Message);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding medical record to patient {PatientId}", patientId);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                ApiResponse<MedicalRecordDto>.ErrorResult("An unexpected error occurred"));
        }
    }
}
