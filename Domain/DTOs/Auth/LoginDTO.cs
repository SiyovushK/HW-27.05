using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.Auth;

public class LoginDTO
{
    [Required(ErrorMessage = "Email is required")]
    [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "Password is required")]
    [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
    public string Password { get; set; } = string.Empty;
}