using AuthService.Domain.Constants;
using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Interfaces;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Domain.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.Permissions.Commands.RevokePermissionFromGroup;

public class RevokePermissionFromGroupCommandHandler 
    : IRequestHandler<RevokePermissionFromGroupCommand, RevokePermissionFromGroupResult>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IGroupPermissionRepository _groupPermissionRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IServerAddress _serverAddress;
    private readonly ILogger<RevokePermissionFromGroupCommandHandler> _logger;

    public RevokePermissionFromGroupCommandHandler(
        IPermissionRepository permissionRepository,
        IGroupRepository groupRepository,
        IGroupPermissionRepository groupPermissionRepository,
        IAuditLogRepository auditLogRepository,
        IServerAddress serverAddress,
        ILogger<RevokePermissionFromGroupCommandHandler> logger)
    {
        _permissionRepository = permissionRepository;
        _groupRepository = groupRepository;
        _groupPermissionRepository = groupPermissionRepository;
        _auditLogRepository = auditLogRepository;
        _serverAddress = serverAddress;
        _logger = logger;
    }

    public async Task<RevokePermissionFromGroupResult> Handle(
        RevokePermissionFromGroupCommand request, 
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

        // Check if assignment exists
        if (!await _groupPermissionRepository.ExistsAsync(request.GroupId, request.PermissionId))
        {
            throw new InvalidOperationException(
                $"Permission '{permission.Key}' is not assigned to group '{group.Name}'.");
        }

        // Revoke the assignment
        await _groupPermissionRepository.DeleteByGroupAndPermissionAsync(request.GroupId, request.PermissionId);

        _logger.LogInformation(
            "Permission {PermissionKey} revoked from group {GroupName} by user {UserId}",
            permission.Key,
            group.Name,
            request.RevokedByUserId);

        // Audit log
        var auditLog = AuditLog.Create(
            request.RevokedByUserId,
            AuditLogActions.PermissionRevoked,
            $"Permission '{permission.Key}' revoked from group '{group.Name}'",
            await _serverAddress.GetCurrentIPv4ServerAddress()
        );
        await _auditLogRepository.AddAsync(auditLog);

        return new RevokePermissionFromGroupResult(
            true,
            $"Permission '{permission.Key}' successfully revoked from group '{group.Name}'"
        );
    }
}
