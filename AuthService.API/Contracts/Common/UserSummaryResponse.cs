namespace AuthService.API.Contracts.Common;

public record UserSummaryResponse(
    Guid Id,
    string UserName,
    string Email);
