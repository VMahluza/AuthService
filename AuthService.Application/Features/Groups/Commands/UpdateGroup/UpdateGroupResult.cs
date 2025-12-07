namespace AuthService.Application.Features.Groups.Commands.UpdateGroup;

public record UpdateGroupResult(
    Guid GroupId,
    string Name,
    string Description,
    DateTime LastUpdatedAt
);
