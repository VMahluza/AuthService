namespace AuthService.API.Contracts.Auth.Login;

public record LoginUserRequest(
    string UserName,
    string Password
    );
