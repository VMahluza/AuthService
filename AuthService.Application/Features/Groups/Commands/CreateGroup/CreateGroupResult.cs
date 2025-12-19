namespace AuthService.Application.Features.Groups.Commands.CreateGroup;

public record CreateGroupResult(
    Guid GroupId,
    string Name,
    string Description,
    DateTime CreatedAt
);
