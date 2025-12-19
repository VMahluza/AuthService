using AuthService.Domain.DTOs;

namespace AuthService.Application.Features.Permissions.Queries.GetUserPermissions;

public record GetUserPermissionsResult(
    Guid UserId,
    string UserName,
    IEnumerable<PermissionDTO> Permissions
);

public record PermissionDTO(
    Guid Id,
    string Key,
    string Name,
    string Description
);
