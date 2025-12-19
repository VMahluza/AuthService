using MediatR;

namespace AuthService.Application.Features.Groups.Queries.GetGroups;

public record GetGroupsQuery(
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<GetGroupsResult>;
