using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartCare.API.Auth;
using SmartCare.Application.Common.DTOs;
using SmartCare.Infrastructure.Persistence;
using SmartCare.Infrastructure.Persistence.Entities;

namespace SmartCare.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly SmartCareDbContext _dbContext;
    private readonly PasswordHasher<UserEntity> _passwordHasher;

    public AuthController(ITokenService tokenService, SmartCareDbContext dbContext)
    {
        _tokenService = tokenService;
        _dbContext = dbContext;
        _passwordHasher = new PasswordHasher<UserEntity>();
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

        if (user == null)
        {
            return Unauthorized(ApiResponse<LoginResponse>.ErrorResult("Invalid credentials"));
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return Unauthorized(ApiResponse<LoginResponse>.ErrorResult("Invalid credentials"));
        }

        var token = _tokenService.CreateToken(user.Email, user.Role);
        return Ok(ApiResponse<LoginResponse>.SuccessResult(token, "Login successful"));
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(ApiResponse<LoginResponse>.ErrorResult("Email and password are required"));
        }

        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());
        if (existingUser != null)
        {
            return BadRequest(ApiResponse<LoginResponse>.ErrorResult("User with this email already exists"));
        }

        var role = AppRoles.Patient;
        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            // Admin role can only be assigned through the admin/create-user endpoint
            if (request.Role.Equals(AppRoles.Doctor, StringComparison.OrdinalIgnoreCase)) role = AppRoles.Doctor;
            else if (request.Role.Equals(AppRoles.Nurse, StringComparison.OrdinalIgnoreCase)) role = AppRoles.Nurse;
            // Admin intentionally excluded — self-registration cannot grant admin access
        }

        var newUser = new UserEntity
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Role = role,
            CreatedAt = DateTime.UtcNow
        };

        newUser.PasswordHash = _passwordHasher.HashPassword(newUser, request.Password);

        _dbContext.Users.Add(newUser);
        await _dbContext.SaveChangesAsync();

        var token = _tokenService.CreateToken(newUser.Email, newUser.Role);
        return Ok(ApiResponse<LoginResponse>.SuccessResult(token, "Registration successful"));
    }

    [Authorize]
    [HttpPost("admin/create-user")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> CreateUserByAdmin([FromBody] CreateUserRequest request)
    {
        var currentUserEmail = User.Identity?.Name;
        var currentUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == (currentUserEmail ?? string.Empty).ToLower());

        if (currentUser == null || !string.Equals(currentUser.Role, AppRoles.Admin, StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<LoginResponse>.ErrorResult("Only admins can create users"));
        }

        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(ApiResponse<LoginResponse>.ErrorResult("Email and password are required"));
        }

        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());
        if (existingUser != null)
        {
            return BadRequest(ApiResponse<LoginResponse>.ErrorResult("User with this email already exists"));
        }

        var role = AppRoles.Patient;
        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            if (request.Role.Equals(AppRoles.Admin, StringComparison.OrdinalIgnoreCase)) role = AppRoles.Admin;
            else if (request.Role.Equals(AppRoles.Doctor, StringComparison.OrdinalIgnoreCase)) role = AppRoles.Doctor;
            else if (request.Role.Equals(AppRoles.Nurse, StringComparison.OrdinalIgnoreCase)) role = AppRoles.Nurse;
        }

        var newUser = new UserEntity
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Role = role,
            CreatedAt = DateTime.UtcNow
        };

        newUser.PasswordHash = _passwordHasher.HashPassword(newUser, request.Password);

        _dbContext.Users.Add(newUser);
        await _dbContext.SaveChangesAsync();

        return Ok(ApiResponse<LoginResponse>.SuccessResult(new LoginResponse(), "User created successfully"));
    }
}
