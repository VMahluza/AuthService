using System.ComponentModel.DataAnnotations;

namespace AuthService.API.Contracts.Roles.UpdateRole;

public record UpdateRoleRequest
{
    [Required, MinLength(2), MaxLength(100)]
    public string Name { get; init; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; init; } = string.Empty;
}
