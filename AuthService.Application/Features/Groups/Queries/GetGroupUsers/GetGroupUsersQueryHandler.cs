using AuthService.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.Groups.Queries.GetGroupUsers;

public class GetGroupUsersQueryHandler : IRequestHandler<GetGroupUsersQuery, GetGroupUsersResult>
{
    private readonly ILogger<GetGroupUsersQueryHandler> _logger;
    private readonly IGroupRepository _groupRepository;
    private readonly IUserGroupRepository _userGroupRepository;

    public GetGroupUsersQueryHandler(
        ILogger<GetGroupUsersQueryHandler> logger,
        IGroupRepository groupRepository,
        IUserGroupRepository userGroupRepository)
    {
        _logger = logger;
        _groupRepository = groupRepository;
        _userGroupRepository = userGroupRepository;
    }

    public async Task<GetGroupUsersResult> Handle(GetGroupUsersQuery request, CancellationToken cancellationToken)
    {
        // Verify group exists
        var group = await _groupRepository.GetByIdAsync(request.GroupId);
        if (group == null)
        {
            throw new InvalidOperationException($"Group with ID '{request.GroupId}' not found.");
        }

        // Get users in this group
        var users = await _userGroupRepository.GetUsersByGroupIdAsync(request.GroupId);
        var usersList = users.ToList();

        // Map to DTOs
        var userDtos = usersList.Select(user => new UserDto(
            user.Id,
            user.UserName,
            user.Email.Value
        ));

        _logger.LogInformation(
            "Retrieved {Count} users for group '{GroupName}' (ID: {GroupId})",
            usersList.Count, group.Name, group.Id);

        return new GetGroupUsersResult(
            group.Id,
            group.Name,
            userDtos
        );
    }
}
