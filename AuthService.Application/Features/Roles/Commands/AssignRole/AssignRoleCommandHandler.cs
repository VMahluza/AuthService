using AuthService.Domain.Constants;
using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Entities.User;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Domain.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.Roles.Commands.AssignRole;

public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, AssignRoleResult>
{
    private readonly ILogger<AssignRoleCommandHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IServerAddress _serverAddress;

    public AssignRoleCommandHandler(
        ILogger<AssignRoleCommandHandler> logger,
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

    public async Task<AssignRoleResult> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
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

        // Check if user already has this role
        var hasRole = await _userRoleRepository.HasRoleAsync(request.UserId, request.RoleId);
        if (hasRole)
        {
            throw new InvalidOperationException(
                $"User '{user.UserName}' already has role '{role.Name}'.");
        }

        // Assign role to user
        var userRole = new UserRole(request.UserId, request.RoleId);
        await _userRoleRepository.AddAsync(userRole);

        // Log the assignment
        await LogRoleAssignment(user.Id, user.UserName, role.Id, role.Name);

        _logger.LogInformation(
            "Role '{RoleName}' assigned to user '{UserName}' (ID: {UserId})",
            role.Name, user.UserName, user.Id);

        return new AssignRoleResult(
            true,
            $"Role '{role.Name}' successfully assigned to user '{user.UserName}'",
            user.Id,
            role.Id,
            role.Name
        );
    }

    private async Task LogRoleAssignment(Guid userId, string userName, Guid roleId, string roleName)
    {
        try
        {
            var ipAddress = await _serverAddress.GetCurrentIPv4ServerAddress();
            var auditLog = new AuditLog(
                Guid.NewGuid(),
                userId,
                AuditLogActions.RoleAssignedToUser,
                $"Role '{roleName}' assigned to user '{userName}'",
                ipAddress
            );

            await _auditLogRepository.AddAsync(auditLog);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log role assignment audit");
        }
    }
}
