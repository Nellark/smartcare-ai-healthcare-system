using System.ComponentModel.DataAnnotations;

namespace SmartCare.API.Auth;

public class RegisterRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(256, ErrorMessage = "Email must be 256 characters or fewer")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [MaxLength(128, ErrorMessage = "Password must be 128 characters or fewer")]
    public string Password { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}
