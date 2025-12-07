using System.ComponentModel.DataAnnotations;

namespace AuthService.API.Contracts;

public record ForgotPasswordRequest
{
    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; init; } = string.Empty;
}
