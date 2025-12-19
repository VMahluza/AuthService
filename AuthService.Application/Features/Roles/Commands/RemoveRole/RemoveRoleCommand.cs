using MediatR;

namespace AuthService.Application.Features.Roles.Commands.RemoveRole;

public record RemoveRoleCommand(
    Guid UserId,
    Guid RoleId
) : IRequest<RemoveRoleResult>;
