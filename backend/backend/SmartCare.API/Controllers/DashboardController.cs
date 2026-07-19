using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCare.Application.Common.DTOs;
using SmartCare.Application.Dashboard.Queries;

namespace SmartCare.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IMediator mediator, ILogger<DashboardController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("stats")]
    [Authorize(Policy = "ViewDashboard")]
    [ProducesResponseType(typeof(ApiResponse<DashboardStatsDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<DashboardStatsDto>>> GetStats()
    {
        try
        {
            var result = await _mediator.Send(new GetDashboardStatsQuery());
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dashboard statistics");
            return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<DashboardStatsDto>.ErrorResult("An unexpected error occurred"));
        }
    }
}
