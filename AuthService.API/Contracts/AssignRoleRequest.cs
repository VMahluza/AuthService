using System.ComponentModel.DataAnnotations;

namespace AuthService.API.Contracts;

public record AssignRoleRequest
{
    [Required]
    public Guid UserId { get; init; }

    [Required]
    public Guid RoleId { get; init; }
}
