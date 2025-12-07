namespace AuthService.API.Contracts;

public record AddUserToGroupResponse(
    bool Success,
    string Message,
    Guid UserId,
    Guid GroupId,
    string GroupName
);
