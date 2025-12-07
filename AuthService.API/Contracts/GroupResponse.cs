namespace AuthService.API.Contracts;

public record GroupResponse(
    Guid Id,
    string Name,
    string Description,
    DateTime CreatedAt,
    DateTime? LastUpdatedAt
);
