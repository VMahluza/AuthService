namespace AuthService.API.Contracts;

public record GroupUsersResponse(
    Guid GroupId,
    string GroupName,
    IEnumerable<UserSummaryResponse> Users
);
