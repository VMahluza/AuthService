namespace AuthService.API.Contracts.Groups.AddUserToGroup;

public record AddUserToGroupResponse(
    bool Success,
    string Message,
    Guid UserId,
    Guid GroupId,
    string GroupName
);
