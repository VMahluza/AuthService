using AuthService.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.Groups.Queries.GetGroups;

public class GetGroupsQueryHandler : IRequestHandler<GetGroupsQuery, GetGroupsResult>
{
    private readonly ILogger<GetGroupsQueryHandler> _logger;
    private readonly IGroupRepository _groupRepository;

    public GetGroupsQueryHandler(
        ILogger<GetGroupsQueryHandler> logger,
        IGroupRepository groupRepository)
    {
        _logger = logger;
        _groupRepository = groupRepository;
    }

    public async Task<GetGroupsResult> Handle(GetGroupsQuery request, CancellationToken cancellationToken)
    {
        // Get paginated groups
        var groups = await _groupRepository.GetAllAsync(request.PageNumber, request.PageSize);
        var groupsList = groups.ToList();

        // Map to DTOs
        var groupDtos = groupsList.Select(group => new GroupDto(
            group.Id,
            group.Name,
            group.Description,
            group.CreatedAt,
            group.LastUpdatedAt
        ));

        _logger.LogInformation(
            "Retrieved {Count} groups for page {PageNumber} with page size {PageSize}",
            groupsList.Count, request.PageNumber, request.PageSize);

        return new GetGroupsResult(
            groupDtos,
            groupsList.Count, // In production, this should be total count from DB
            request.PageNumber,
            request.PageSize
        );
    }
}
