using AuthService.Domain.Constants;
using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Interfaces;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Domain.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.Permissions.Commands.AssignPermissionToGroup;

public class AssignPermissionToGroupCommandHandler 
    : IRequestHandler<AssignPermissionToGroupCommand, AssignPermissionToGroupResult>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IGroupPermissionRepository _groupPermissionRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IServerAddress _serverAddress;
    private readonly ILogger<AssignPermissionToGroupCommandHandler> _logger;

    public AssignPermissionToGroupCommandHandler(
        IPermissionRepository permissionRepository,
        IGroupRepository groupRepository,
        IGroupPermissionRepository groupPermissionRepository,
        IAuditLogRepository auditLogRepository,
        IServerAddress serverAddress,
        ILogger<AssignPermissionToGroupCommandHandler> logger)
    {
        _permissionRepository = permissionRepository;
        _groupRepository = groupRepository;
        _groupPermissionRepository = groupPermissionRepository;
        _auditLogRepository = auditLogRepository;
        _serverAddress = serverAddress;
        _logger = logger;
    }

    public async Task<AssignPermissionToGroupResult> Handle(
        AssignPermissionToGroupCommand request, 
        CancellationToken cancellationToken)
    {
        // Validate group exists
        var group = await _groupRepository.GetByIdAsync(request.GroupId);
        if (group == null)
        {
            throw new InvalidOperationException($"Group with ID '{request.GroupId}' not found.");
        }

        // Validate permission exists
        var permission = await _permissionRepository.GetByIdAsync(request.PermissionId);
        if (permission == null)
        {
            throw new InvalidOperationException($"Permission with ID '{request.PermissionId}' not found.");
        }

        // Check if assignment already exists
        if (await _groupPermissionRepository.ExistsAsync(request.GroupId, request.PermissionId))
        {
            throw new InvalidOperationException(
                $"Permission '{permission.Key}' is already assigned to group '{group.Name}'.");
        }

        // Create the assignment
        var groupPermission = GroupPermission.Create(
            request.GroupId,
            request.PermissionId,
            request.AssignedByUserId);

        await _groupPermissionRepository.AddAsync(groupPermission);

        _logger.LogInformation(
            "Permission {PermissionKey} assigned to group {GroupName} by user {UserId}",
            permission.Key,
            group.Name,
            request.AssignedByUserId);

        // Audit log
        var auditLog = AuditLog.Create(
            request.AssignedByUserId,
            AuditLogActions.PermissionAssigned,
            $"Permission '{permission.Key}' assigned to group '{group.Name}'",
            await _serverAddress.GetCurrentIPv4ServerAddress()
        );
        await _auditLogRepository.AddAsync(auditLog);

        return new AssignPermissionToGroupResult(
            true,
            $"Permission '{permission.Key}' successfully assigned to group '{group.Name}'",
            request.GroupId,
            request.PermissionId,
            permission.Key
        );
    }
}
