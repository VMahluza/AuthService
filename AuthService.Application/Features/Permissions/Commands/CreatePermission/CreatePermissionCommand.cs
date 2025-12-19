using MediatR;

namespace AuthService.Application.Features.Permissions.Commands.CreatePermission;

public record CreatePermissionCommand(
    string Key,
    string Name,
    string Description
) : IRequest<CreatePermissionResult>;
