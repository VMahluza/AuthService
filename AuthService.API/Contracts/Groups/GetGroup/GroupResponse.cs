namespace AuthService.API.Contracts.Groups.GetGroup;

public record GroupResponse(
    Guid Id,
    string Name,
    string Description,
    DateTime CreatedAt,
    DateTime? LastUpdatedAt
);
