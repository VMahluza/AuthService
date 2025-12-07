using AuthService.Domain.Constants;
using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Entities.User;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Domain.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.Roles.Commands.CreateRole;

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, CreateRoleResult>
{
    private readonly ILogger<CreateRoleCommandHandler> _logger;
    private readonly IRoleRepository _roleRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IServerAddress _serverAddress;

    public CreateRoleCommandHandler(
        ILogger<CreateRoleCommandHandler> logger,
        IRoleRepository roleRepository,
        IAuditLogRepository auditLogRepository,
        IServerAddress serverAddress)
    {
        _logger = logger;
        _roleRepository = roleRepository;
        _auditLogRepository = auditLogRepository;
        _serverAddress = serverAddress;
    }

    public async Task<CreateRoleResult> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        // Check if role name already exists
        if (await _roleRepository.ExistsByNameAsync(request.Name))
        {
            throw new InvalidOperationException($"Role with name '{request.Name}' already exists.");
        }

        // Create new role
        var role = Role.Create(request.Name, request.Description);
        await _roleRepository.AddAsync(role);

        // Log the creation
        await LogRoleCreation(role);

        _logger.LogInformation("Role '{RoleName}' created with ID: {RoleId}", role.Name, role.Id);

        return new CreateRoleResult(
            role.Id,
            role.Name,
            role.Description,
            role.CreatedAt
        );
    }

    private async Task LogRoleCreation(Role role)
    {
        try
        {
            var ipAddress = await _serverAddress.GetCurrentIPv4ServerAddress();
            var auditLog = new AuditLog(
                Guid.NewGuid(),
                Guid.Empty, // System action - or pass in the admin user ID if available
                AuditLogActions.RoleCreated,
                $"Role '{role.Name}' created",
                ipAddress
            );

            await _auditLogRepository.AddAsync(auditLog);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log role creation audit");
        }
    }
}
