using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.Auth;

public class RegisterDTO
{
    public string FullName { get; set; } = string.Empty;
    [StringLength(20, ErrorMessage = "Must be between 9 numbers or more", MinimumLength = 9)]
    public string Phone { get; set; } = string.Empty;
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}