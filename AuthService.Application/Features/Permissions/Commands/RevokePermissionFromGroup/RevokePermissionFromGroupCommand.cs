using MediatR;

namespace AuthService.Application.Features.Permissions.Commands.RevokePermissionFromGroup;

public record RevokePermissionFromGroupCommand(
    Guid GroupId,
    Guid PermissionId,
    Guid RevokedByUserId
) : IRequest<RevokePermissionFromGroupResult>;
