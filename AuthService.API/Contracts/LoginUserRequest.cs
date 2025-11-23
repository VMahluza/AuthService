namespace AuthService.API.Contracts;

public record LoginUserRequest(
    string UserName,
    string Password
    );
