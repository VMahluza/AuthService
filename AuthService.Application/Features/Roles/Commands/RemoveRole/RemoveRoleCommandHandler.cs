using AuthService.Domain.Constants;
using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Domain.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.Roles.Commands.RemoveRole;

public class RemoveRoleCommandHandler : IRequestHandler<RemoveRoleCommand, RemoveRoleResult>
{
    private readonly ILogger<RemoveRoleCommandHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IServerAddress _serverAddress;

    public RemoveRoleCommandHandler(
        ILogger<RemoveRoleCommandHandler> logger,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IAuditLogRepository auditLogRepository,
        IServerAddress serverAddress)
    {
        _logger = logger;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _auditLogRepository = auditLogRepository;
        _serverAddress = serverAddress;
    }

    public async Task<RemoveRoleResult> Handle(RemoveRoleCommand request, CancellationToken cancellationToken)
    {
        // Verify user exists
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID '{request.UserId}' not found.");
        }

        // Verify role exists
        var role = await _roleRepository.GetByIdAsync(request.RoleId);
        if (role == null)
        {
            throw new InvalidOperationException($"Role with ID '{request.RoleId}' not found.");
        }

        // Check if user has this role
        var hasRole = await _userRoleRepository.HasRoleAsync(request.UserId, request.RoleId);
        if (!hasRole)
        {
            throw new InvalidOperationException(
                $"User '{user.UserName}' does not have role '{role.Name}'.");
        }

        // Remove role from user
        await _userRoleRepository.RemoveAsync(request.UserId, request.RoleId);

        // Log the removal
        await LogRoleRemoval(user.Id, user.UserName, role.Id, role.Name);

        _logger.LogInformation(
            "Role '{RoleName}' removed from user '{UserName}' (ID: {UserId})",
            role.Name, user.UserName, user.Id);

        return new RemoveRoleResult(
            true,
            $"Role '{role.Name}' successfully removed from user '{user.UserName}'"
        );
    }

    private async Task LogRoleRemoval(Guid userId, string userName, Guid roleId, string roleName)
    {
        try
        {
            var ipAddress = await _serverAddress.GetCurrentIPv4ServerAddress();
            var auditLog = new AuditLog(
                Guid.NewGuid(),
                userId,
                AuditLogActions.RoleRemovedFromUser,
                $"Role '{roleName}' removed from user '{userName}'",
                ipAddress
            );

            await _auditLogRepository.AddAsync(auditLog);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log role removal audit");
        }
    }
}
