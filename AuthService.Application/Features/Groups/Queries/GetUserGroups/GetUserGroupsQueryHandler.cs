using AuthService.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.Groups.Queries.GetUserGroups;

public class GetUserGroupsQueryHandler : IRequestHandler<GetUserGroupsQuery, GetUserGroupsResult>
{
    private readonly ILogger<GetUserGroupsQueryHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IUserGroupRepository _userGroupRepository;

    public GetUserGroupsQueryHandler(
        ILogger<GetUserGroupsQueryHandler> logger,
        IUserRepository userRepository,
        IUserGroupRepository userGroupRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _userGroupRepository = userGroupRepository;
    }

    public async Task<GetUserGroupsResult> Handle(GetUserGroupsQuery request, CancellationToken cancellationToken)
    {
        // Verify user exists
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID '{request.UserId}' not found.");
        }

        // Get user's groups
        var groups = await _userGroupRepository.GetGroupsByUserIdAsync(request.UserId);
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
            "Retrieved {Count} groups for user '{UserName}' (ID: {UserId})",
            groupsList.Count, user.UserName, user.Id);

        return new GetUserGroupsResult(
            user.Id,
            user.UserName,
            groupDtos
        );
    }
}
