using AuthService.Domain.Constants;
using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Domain.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.Groups.Commands.RemoveUserFromGroup;

public class RemoveUserFromGroupCommandHandler : IRequestHandler<RemoveUserFromGroupCommand, RemoveUserFromGroupResult>
{
    private readonly ILogger<RemoveUserFromGroupCommandHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUserGroupRepository _userGroupRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IServerAddress _serverAddress;

    public RemoveUserFromGroupCommandHandler(
        ILogger<RemoveUserFromGroupCommandHandler> logger,
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

    public async Task<RemoveUserFromGroupResult> Handle(RemoveUserFromGroupCommand request, CancellationToken cancellationToken)
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

        // Check if user is a member of this group
        var isMember = await _userGroupRepository.IsMemberAsync(request.UserId, request.GroupId);
        if (!isMember)
        {
            throw new InvalidOperationException(
                $"User '{user.UserName}' is not a member of group '{group.Name}'.");
        }

        // Remove user from group
        await _userGroupRepository.RemoveAsync(request.UserId, request.GroupId);

        // Log the removal
        await LogUserRemovedFromGroup(user.Id, user.UserName, group.Id, group.Name);

        _logger.LogInformation(
            "User '{UserName}' (ID: {UserId}) removed from group '{GroupName}'",
            user.UserName, user.Id, group.Name);

        return new RemoveUserFromGroupResult(
            true,
            $"User '{user.UserName}' successfully removed from group '{group.Name}'"
        );
    }

    private async Task LogUserRemovedFromGroup(Guid userId, string userName, Guid groupId, string groupName)
    {
        try
        {
            var ipAddress = await _serverAddress.GetCurrentIPv4ServerAddress();
            var auditLog = new AuditLog(
                Guid.NewGuid(),
                userId,
                AuditLogActions.UserRemovedFromGroup,
                $"User '{userName}' removed from group '{groupName}'",
                ipAddress
            );

            await _auditLogRepository.AddAsync(auditLog);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log user removed from group audit");
        }
    }
}
