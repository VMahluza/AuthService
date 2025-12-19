using MediatR;

namespace AuthService.Application.Features.Roles.Commands.UpdateRole;

public record UpdateRoleCommand(
    Guid RoleId,
    string Name,
    string Description
) : IRequest<UpdateRoleResult>;
