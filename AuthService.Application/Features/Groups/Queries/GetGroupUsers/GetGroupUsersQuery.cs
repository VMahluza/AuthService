using MediatR;

namespace AuthService.Application.Features.Groups.Queries.GetGroupUsers;

public record GetGroupUsersQuery(
    Guid GroupId
) : IRequest<GetGroupUsersResult>;
