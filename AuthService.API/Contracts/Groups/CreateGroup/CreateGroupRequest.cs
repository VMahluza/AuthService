using System.ComponentModel.DataAnnotations;

namespace AuthService.API.Contracts.Groups.CreateGroup;

public record CreateGroupRequest
{
    [Required, MinLength(2), MaxLength(100)]
    public string Name { get; init; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; init; } = string.Empty;
}
