namespace AuthService.Application.Features.Permissions.Commands.RevokePermissionFromGroup;

public record RevokePermissionFromGroupResult(
    bool Success,
    string Message
);
