using AuthService.Domain.Constants;
using AuthService.Domain.Entities.User;
using AuthService.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.Permissions.Commands.CreatePermission;

public class CreatePermissionCommandHandler : IRequestHandler<CreatePermissionCommand, CreatePermissionResult>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly ILogger<CreatePermissionCommandHandler> _logger;

    public CreatePermissionCommandHandler(
        IPermissionRepository permissionRepository,
        IAuditLogRepository auditLogRepository,
        ILogger<CreatePermissionCommandHandler> logger)
    {
        _permissionRepository = permissionRepository;
        _auditLogRepository = auditLogRepository;
        _logger = logger;
    }

    public async Task<CreatePermissionResult> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
    {
        // Check if permission with this key already exists
        if (await _permissionRepository.ExistsByKeyAsync(request.Key))
        {
            throw new InvalidOperationException($"Permission with key '{request.Key}' already exists.");
        }

        var permission = Permission.Create(request.Key, request.Name, request.Description);
        await _permissionRepository.AddAsync(permission);

        _logger.LogInformation(
            "Permission created: {PermissionKey} ({PermissionId})",
            permission.Key,
            permission.Id);

        // TODO: Add audit log entry once we have current user context

        return new CreatePermissionResult(
            permission.Id,
            permission.Key,
            permission.Name,
            permission.Description,
            permission.CreatedAt
        );
    }
}
