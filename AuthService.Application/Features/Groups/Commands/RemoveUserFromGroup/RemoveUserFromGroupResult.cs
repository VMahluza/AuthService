namespace AuthService.Application.Features.Groups.Commands.RemoveUserFromGroup;

public record RemoveUserFromGroupResult(
    bool Success,
    string Message
);
