using AuthService.Domain.Constants;
using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Domain.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.Roles.Commands.DeleteRole;

public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, DeleteRoleResult>
{
    private readonly ILogger<DeleteRoleCommandHandler> _logger;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IServerAddress _serverAddress;

    public DeleteRoleCommandHandler(
        ILogger<DeleteRoleCommandHandler> logger,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IAuditLogRepository auditLogRepository,
        IServerAddress serverAddress)
    {
        _logger = logger;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _auditLogRepository = auditLogRepository;
        _serverAddress = serverAddress;
    }

    public async Task<DeleteRoleResult> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        // Get the role
        var role = await _roleRepository.GetByIdAsync(request.RoleId);
        if (role == null)
        {
            throw new InvalidOperationException($"Role with ID '{request.RoleId}' not found.");
        }

        // Check if role is assigned to any users
        var userCount = await _roleRepository.GetUserCountAsync(request.RoleId);
        if (userCount > 0)
        {
            throw new InvalidOperationException(
                $"Cannot delete role '{role.Name}' as it is assigned to {userCount} user(s). " +
                "Remove all user assignments before deleting.");
        }

        var roleName = role.Name;

        // Delete the role
        await _roleRepository.DeleteAsync(role);

        // Log the deletion
        await LogRoleDeletion(request.RoleId, roleName);

        _logger.LogInformation("Role '{RoleName}' (ID: {RoleId}) deleted successfully", roleName, request.RoleId);

        return new DeleteRoleResult(
            true,
            $"Role '{roleName}' deleted successfully"
        );
    }

    private async Task LogRoleDeletion(Guid roleId, string roleName)
    {
        try
        {
            var ipAddress = await _serverAddress.GetCurrentIPv4ServerAddress();
            var auditLog = new AuditLog(
                Guid.NewGuid(),
                Guid.Empty, // System action
                AuditLogActions.RoleDeleted,
                $"Role '{roleName}' (ID: {roleId}) deleted",
                ipAddress
            );

            await _auditLogRepository.AddAsync(auditLog);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log role deletion audit");
        }
    }
}
