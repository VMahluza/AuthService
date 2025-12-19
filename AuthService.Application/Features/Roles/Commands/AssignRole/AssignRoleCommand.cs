using MediatR;

namespace AuthService.Application.Features.Roles.Commands.AssignRole;

public record AssignRoleCommand(
    Guid UserId,
    Guid RoleId
) : IRequest<AssignRoleResult>;
