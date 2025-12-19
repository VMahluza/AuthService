namespace AuthService.API.Contracts.Roles.GetRole;

public record RoleResponse(
    Guid Id,
    string Name,
    string Description,
    DateTime CreatedAt,
    DateTime? LastUpdatedAt
);
