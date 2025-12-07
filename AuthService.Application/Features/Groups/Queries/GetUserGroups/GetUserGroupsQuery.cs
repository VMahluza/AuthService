using MediatR;

namespace AuthService.Application.Features.Groups.Queries.GetUserGroups;

public record GetUserGroupsQuery(
    Guid UserId
) : IRequest<GetUserGroupsResult>;
