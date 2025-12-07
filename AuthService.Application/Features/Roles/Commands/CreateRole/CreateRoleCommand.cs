using MediatR;

namespace AuthService.Application.Features.Roles.Commands.CreateRole;

public record CreateRoleCommand(
    string Name,
    string Description
) : IRequest<CreateRoleResult>;
