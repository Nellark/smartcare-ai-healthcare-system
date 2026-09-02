using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using SmartCare.API.Auth;
using SmartCare.API.Services;
using SmartCare.Application.Common.DTOs;
using SmartCare.Infrastructure.Persistence;
using SmartCare.Infrastructure.Persistence.Entities;

namespace SmartCare.API.Controllers;

public sealed class PasswordResetLinkResponse
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string ResetLink { get; set; } = string.Empty;
}

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly SmartCareDbContext _dbContext;
    private readonly PasswordHasher<UserEntity> _passwordHasher;
    private readonly IEmailService _emailService;

    public AuthController(ITokenService tokenService, SmartCareDbContext dbContext, IEmailService emailService)
    {
        _tokenService = tokenService;
        _dbContext = dbContext;
        _passwordHasher = new PasswordHasher<UserEntity>();
        _emailService = emailService;
    }

    [HttpPost("login")]
    [EnableRateLimiting("auth-login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        // Use a generic error message to prevent user enumeration attacks
        const string genericError = "Invalid credentials";

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

        if (user == null)
        {
            // Constant-time response to prevent timing attacks
            _passwordHasher.VerifyHashedPassword(new UserEntity { PasswordHash = string.Empty }, "dummyhash", request.Password);
            return Unauthorized(ApiResponse<LoginResponse>.ErrorResult(genericError));
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return Unauthorized(ApiResponse<LoginResponse>.ErrorResult(genericError));
        }

        // Rehash if password was using an older algorithm
        if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            await _dbContext.SaveChangesAsync();
        }

        var token = _tokenService.CreateToken(user.Email, user.Role);
        return Ok(ApiResponse<LoginResponse>.SuccessResult(token, "Login successful"));
    }

    [HttpPost("register")]
    [EnableRateLimiting("auth-register")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(ApiResponse<LoginResponse>.ErrorResult("Email and password are required"));
        }

        // Enforce password complexity
        var passwordValidation = PasswordPolicy.Validate(request.Password);
        if (!passwordValidation.IsValid)
        {
            return BadRequest(ApiResponse<LoginResponse>.ErrorResult("Password does not meet security requirements", passwordValidation.Errors));
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

        // Enforce password complexity on admin-created users too
        var passwordValidation = PasswordPolicy.Validate(request.Password);
        if (!passwordValidation.IsValid)
        {
            return BadRequest(ApiResponse<LoginResponse>.ErrorResult("Password does not meet security requirements", passwordValidation.Errors));
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

    [HttpPost("forgot-password")]
    [EnableRateLimiting("auth-login")]
    [ProducesResponseType(typeof(ApiResponse<PasswordResetLinkResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PasswordResetLinkResponse>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PasswordResetLinkResponse>>> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(ApiResponse<PasswordResetLinkResponse>.ErrorResult("Email is required"));
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());
        if (user == null)
        {
            return Ok(ApiResponse<PasswordResetLinkResponse>.SuccessResult(
                new PasswordResetLinkResponse
                {
                    Email = request.Email,
                    Token = string.Empty,
                    ResetLink = string.Empty
                },
                "If that email is in our system, we have sent a reset link to it."));
        }

        var resetToken = Guid.NewGuid().ToString("N");
        user.ResetToken = resetToken;
        user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
        await _dbContext.SaveChangesAsync();

        var frontendBaseUrl = Environment.GetEnvironmentVariable("SMARTCARE_FRONTEND_URL") ?? "http://localhost:4200";
        var resetLink = $"{frontendBaseUrl}/forgot-password?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(resetToken)}";

        try
        {
            await _emailService.SendPasswordResetEmailAsync(user.Email, resetLink);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<PasswordResetLinkResponse>.ErrorResult("Unable to send password reset email right now. Please try again later.", new List<string> { ex.Message }));
        }

        var response = new PasswordResetLinkResponse
        {
            Email = user.Email,
            Token = resetToken,
            ResetLink = resetLink
        };

        return Ok(ApiResponse<PasswordResetLinkResponse>.SuccessResult(
            response,
            "If that email is in our system, we have sent a reset link to it."));
    }

    [HttpPost("reset-password")]
    [EnableRateLimiting("auth-login")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<string>>> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.NewPassword))
        {
            return BadRequest(ApiResponse<string>.ErrorResult("Email, token, and new password are required"));
        }

        var passwordValidation = PasswordPolicy.Validate(request.NewPassword);
        if (!passwordValidation.IsValid)
        {
            return BadRequest(ApiResponse<string>.ErrorResult("Password does not meet security requirements", passwordValidation.Errors));
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());
        if (user == null || user.ResetToken != request.Token || user.ResetTokenExpiry < DateTime.UtcNow)
        {
            return BadRequest(ApiResponse<string>.ErrorResult("Invalid or expired reset token"));
        }

        user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
        user.ResetToken = null;
        user.ResetTokenExpiry = null;
        await _dbContext.SaveChangesAsync();

        return Ok(ApiResponse<string>.SuccessResult(string.Empty, "Password has been successfully reset"));
    }
}
