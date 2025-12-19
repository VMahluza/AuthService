using MediatR;

namespace AuthService.Application.Features.Roles.Queries.GetUserRoles;

public record GetUserRolesQuery(
    Guid UserId
) : IRequest<GetUserRolesResult>;
