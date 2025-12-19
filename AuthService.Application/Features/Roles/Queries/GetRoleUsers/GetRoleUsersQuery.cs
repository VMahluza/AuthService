using MediatR;

namespace AuthService.Application.Features.Roles.Queries.GetRoleUsers;

public record GetRoleUsersQuery(
    Guid RoleId
) : IRequest<GetRoleUsersResult>;
