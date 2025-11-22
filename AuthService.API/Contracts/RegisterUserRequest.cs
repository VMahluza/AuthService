namespace AuthService.API.Contracts;
public record RegisterUserRequest(
    string UserName,
    string Email,
    string Password
);