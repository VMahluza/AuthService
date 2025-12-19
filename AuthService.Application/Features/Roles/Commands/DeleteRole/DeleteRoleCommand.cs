using MediatR;

namespace AuthService.Application.Features.Roles.Commands.DeleteRole;

public record DeleteRoleCommand(
    Guid RoleId
) : IRequest<DeleteRoleResult>;
