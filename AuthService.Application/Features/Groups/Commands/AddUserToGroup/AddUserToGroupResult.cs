namespace AuthService.Application.Features.Groups.Commands.AddUserToGroup;

public record AddUserToGroupResult(
    bool Success,
    string Message,
    Guid UserId,
    Guid GroupId,
    string GroupName
);
