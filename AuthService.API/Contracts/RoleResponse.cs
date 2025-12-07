namespace AuthService.API.Contracts;

public record RoleResponse(
    Guid Id,
    string Name,
    string Description,
    DateTime CreatedAt,
    DateTime? LastUpdatedAt
);
