using System.ComponentModel.DataAnnotations;

namespace AuthService.API.Contracts.Roles.AssignRole;

public record AssignRoleRequest
{
    [Required]
    public Guid UserId { get; init; }

    [Required]
    public Guid RoleId { get; init; }
}
