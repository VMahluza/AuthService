using AuthService.Domain.Constants;
using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Entities.User;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Domain.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.Roles.Commands.UpdateRole;

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, UpdateRoleResult>
{
    private readonly ILogger<UpdateRoleCommandHandler> _logger;
    private readonly IRoleRepository _roleRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IServerAddress _serverAddress;

    public UpdateRoleCommandHandler(
        ILogger<UpdateRoleCommandHandler> logger,
        IRoleRepository roleRepository,
        IAuditLogRepository auditLogRepository,
        IServerAddress serverAddress)
    {
        _logger = logger;
        _roleRepository = roleRepository;
        _auditLogRepository = auditLogRepository;
        _serverAddress = serverAddress;
    }

    public async Task<UpdateRoleResult> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        // Get existing role
        var role = await _roleRepository.GetByIdAsync(request.RoleId);
        if (role == null)
        {
            throw new InvalidOperationException($"Role with ID '{request.RoleId}' not found.");
        }

        // Check if new name conflicts with another role
        if (role.Name != request.Name)
        {
            var existingRole = await _roleRepository.GetByNameAsync(request.Name);
            if (existingRole != null && existingRole.Id != request.RoleId)
            {
                throw new InvalidOperationException($"Role with name '{request.Name}' already exists.");
            }
        }

        // Update role details
        role.UpdateDetails(request.Name, request.Description);
        await _roleRepository.UpdateAsync(role);

        // Log the update
        await LogRoleUpdate(role);

        _logger.LogInformation("Role '{RoleName}' (ID: {RoleId}) updated successfully", role.Name, role.Id);

        return new UpdateRoleResult(
            role.Id,
            role.Name,
            role.Description,
            role.LastUpdatedAt ?? DateTime.UtcNow
        );
    }

    private async Task LogRoleUpdate(Role role)
    {
        try
        {
            var ipAddress = await _serverAddress.GetCurrentIPv4ServerAddress();
            var auditLog = new AuditLog(
                Guid.NewGuid(),
                Guid.Empty, // System action - or pass in the admin user ID if available
                AuditLogActions.RoleUpdated,
                $"Role '{role.Name}' updated",
                ipAddress
            );

            await _auditLogRepository.AddAsync(auditLog);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log role update audit");
        }
    }
}
