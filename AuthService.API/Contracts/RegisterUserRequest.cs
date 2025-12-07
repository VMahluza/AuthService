using System.ComponentModel.DataAnnotations;

namespace AuthService.API.Contracts;

public record RegisterUserRequest
{
    [Required, MinLength(3), MaxLength(50)]
    public string UserName { get; init; } = string.Empty;

    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; init; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; init; } = string.Empty;

    [Required, Compare(nameof(Password))]
    public string ConfirmPassword { get; init; } = string.Empty;
}