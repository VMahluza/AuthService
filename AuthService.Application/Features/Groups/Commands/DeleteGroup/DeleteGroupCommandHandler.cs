using AuthService.Domain.Constants;
using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Domain.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.Groups.Commands.DeleteGroup;

public class DeleteGroupCommandHandler : IRequestHandler<DeleteGroupCommand, DeleteGroupResult>
{
    private readonly ILogger<DeleteGroupCommandHandler> _logger;
    private readonly IGroupRepository _groupRepository;
    private readonly IUserGroupRepository _userGroupRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IServerAddress _serverAddress;

    public DeleteGroupCommandHandler(
        ILogger<DeleteGroupCommandHandler> logger,
        IGroupRepository groupRepository,
        IUserGroupRepository userGroupRepository,
        IAuditLogRepository auditLogRepository,
        IServerAddress serverAddress)
    {
        _logger = logger;
        _groupRepository = groupRepository;
        _userGroupRepository = userGroupRepository;
        _auditLogRepository = auditLogRepository;
        _serverAddress = serverAddress;
    }

    public async Task<DeleteGroupResult> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        // Get the group
        var group = await _groupRepository.GetByIdAsync(request.GroupId);
        if (group == null)
        {
            throw new InvalidOperationException($"Group with ID '{request.GroupId}' not found.");
        }

        // Check if group has any users
        var userCount = await _groupRepository.GetUserCountAsync(request.GroupId);
        if (userCount > 0)
        {
            throw new InvalidOperationException(
                $"Cannot delete group '{group.Name}' as it has {userCount} member(s). " +
                "Remove all members before deleting.");
        }

        var groupName = group.Name;

        // Delete the group
        await _groupRepository.DeleteAsync(group);

        // Log the deletion
        await LogGroupDeletion(request.GroupId, groupName);

        _logger.LogInformation("Group '{GroupName}' (ID: {GroupId}) deleted successfully", groupName, request.GroupId);

        return new DeleteGroupResult(
            true,
            $"Group '{groupName}' deleted successfully"
        );
    }

    private async Task LogGroupDeletion(Guid groupId, string groupName)
    {
        try
        {
            var ipAddress = await _serverAddress.GetCurrentIPv4ServerAddress();
            var auditLog = new AuditLog(
                Guid.NewGuid(),
                Guid.Empty,
                AuditLogActions.GroupDeleted,
                $"Group '{groupName}' (ID: {groupId}) deleted",
                ipAddress
            );

            await _auditLogRepository.AddAsync(auditLog);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log group deletion audit");
        }
    }
}
