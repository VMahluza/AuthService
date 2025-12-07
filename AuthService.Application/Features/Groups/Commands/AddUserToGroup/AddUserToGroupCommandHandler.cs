using AuthService.Domain.Constants;
using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Entities.User;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Domain.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.Groups.Commands.AddUserToGroup;

public class AddUserToGroupCommandHandler : IRequestHandler<AddUserToGroupCommand, AddUserToGroupResult>
{
    private readonly ILogger<AddUserToGroupCommandHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUserGroupRepository _userGroupRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IServerAddress _serverAddress;

    public AddUserToGroupCommandHandler(
        ILogger<AddUserToGroupCommandHandler> logger,
        IUserRepository userRepository,
        IGroupRepository groupRepository,
        IUserGroupRepository userGroupRepository,
        IAuditLogRepository auditLogRepository,
        IServerAddress serverAddress)
    {
        _logger = logger;
        _userRepository = userRepository;
        _groupRepository = groupRepository;
        _userGroupRepository = userGroupRepository;
        _auditLogRepository = auditLogRepository;
        _serverAddress = serverAddress;
    }

    public async Task<AddUserToGroupResult> Handle(AddUserToGroupCommand request, CancellationToken cancellationToken)
    {
        // Verify user exists
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID '{request.UserId}' not found.");
        }

        // Verify group exists
        var group = await _groupRepository.GetByIdAsync(request.GroupId);
        if (group == null)
        {
            throw new InvalidOperationException($"Group with ID '{request.GroupId}' not found.");
        }

        // Check if user is already in this group
        var isMember = await _userGroupRepository.IsMemberAsync(request.UserId, request.GroupId);
        if (isMember)
        {
            throw new InvalidOperationException(
                $"User '{user.UserName}' is already a member of group '{group.Name}'.");
        }

        // Add user to group
        var userGroup = new UserGroup(request.UserId, request.GroupId);
        await _userGroupRepository.AddAsync(userGroup);

        // Log the addition
        await LogUserAddedToGroup(user.Id, user.UserName, group.Id, group.Name);

        _logger.LogInformation(
            "User '{UserName}' (ID: {UserId}) added to group '{GroupName}'",
            user.UserName, user.Id, group.Name);

        return new AddUserToGroupResult(
            true,
            $"User '{user.UserName}' successfully added to group '{group.Name}'",
            user.Id,
            group.Id,
            group.Name
        );
    }

    private async Task LogUserAddedToGroup(Guid userId, string userName, Guid groupId, string groupName)
    {
        try
        {
            var ipAddress = await _serverAddress.GetCurrentIPv4ServerAddress();
            var auditLog = new AuditLog(
                Guid.NewGuid(),
                userId,
                AuditLogActions.UserAddedToGroup,
                $"User '{userName}' added to group '{groupName}'",
                ipAddress
            );

            await _auditLogRepository.AddAsync(auditLog);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log user added to group audit");
        }
    }
}
