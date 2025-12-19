using AuthService.Domain.Constants;
using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Entities.User;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Domain.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.Groups.Commands.CreateGroup;

public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, CreateGroupResult>
{
    private readonly ILogger<CreateGroupCommandHandler> _logger;
    private readonly IGroupRepository _groupRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IServerAddress _serverAddress;

    public CreateGroupCommandHandler(
        ILogger<CreateGroupCommandHandler> logger,
        IGroupRepository groupRepository,
        IAuditLogRepository auditLogRepository,
        IServerAddress serverAddress)
    {
        _logger = logger;
        _groupRepository = groupRepository;
        _auditLogRepository = auditLogRepository;
        _serverAddress = serverAddress;
    }

    public async Task<CreateGroupResult> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        // Check if group name already exists
        if (await _groupRepository.ExistsByNameAsync(request.Name))
        {
            throw new InvalidOperationException($"Group with name '{request.Name}' already exists.");
        }

        // Create new group
        var group = AuthGroup.Create(request.Name, request.Description);
        await _groupRepository.AddAsync(group);

        // Log the creation
        await LogGroupCreation(group);

        _logger.LogInformation("Group '{GroupName}' created with ID: {GroupId}", group.Name, group.Id);

        return new CreateGroupResult(
            group.Id,
            group.Name,
            group.Description,
            group.CreatedAt
        );
    }

    private async Task LogGroupCreation(AuthGroup group)
    {
        try
        {
            var ipAddress = await _serverAddress.GetCurrentIPv4ServerAddress();
            var auditLog = new AuditLog(
                Guid.NewGuid(),
                Guid.Empty, // System action
                AuditLogActions.GroupCreated,
                $"Group '{group.Name}' created",
                ipAddress
            );

            await _auditLogRepository.AddAsync(auditLog);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log group creation audit");
        }
    }
}
