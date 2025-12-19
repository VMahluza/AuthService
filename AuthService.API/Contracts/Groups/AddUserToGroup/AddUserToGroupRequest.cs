using System.ComponentModel.DataAnnotations;

namespace AuthService.API.Contracts.Groups.AddUserToGroup;

public record AddUserToGroupRequest
{
    [Required]
    public Guid UserId { get; init; }

    [Required]
    public Guid GroupId { get; init; }
}
