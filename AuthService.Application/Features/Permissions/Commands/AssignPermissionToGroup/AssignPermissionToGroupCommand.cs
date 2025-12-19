using MediatR;

namespace AuthService.Application.Features.Permissions.Commands.AssignPermissionToGroup;

public record AssignPermissionToGroupCommand(
    Guid GroupId,
    Guid PermissionId,
    Guid AssignedByUserId
) : IRequest<AssignPermissionToGroupResult>;
