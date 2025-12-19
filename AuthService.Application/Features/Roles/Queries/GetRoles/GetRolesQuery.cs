using MediatR;

namespace AuthService.Application.Features.Roles.Queries.GetRoles;

public record GetRolesQuery(
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<GetRolesResult>;
