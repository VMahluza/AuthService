using AuthService.Domain.Constants;
using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Entities.User;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Domain.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.Groups.Commands.UpdateGroup;

public class UpdateGroupCommandHandler : IRequestHandler<UpdateGroupCommand, UpdateGroupResult>
{
    private readonly ILogger<UpdateGroupCommandHandler> _logger;
    private readonly IGroupRepository _groupRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IServerAddress _serverAddress;

    public UpdateGroupCommandHandler(
        ILogger<UpdateGroupCommandHandler> logger,
        IGroupRepository groupRepository,
        IAuditLogRepository auditLogRepository,
        IServerAddress serverAddress)
    {
        _logger = logger;
        _groupRepository = groupRepository;
        _auditLogRepository = auditLogRepository;
        _serverAddress = serverAddress;
    }

    public async Task<UpdateGroupResult> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        // Get existing group
        var group = await _groupRepository.GetByIdAsync(request.GroupId);
        if (group == null)
        {
            throw new InvalidOperationException($"Group with ID '{request.GroupId}' not found.");
        }

        // Check if new name conflicts with another group
        if (group.Name != request.Name)
        {
            var existingGroup = await _groupRepository.GetByNameAsync(request.Name);
            if (existingGroup != null && existingGroup.Id != request.GroupId)
            {
                throw new InvalidOperationException($"Group with name '{request.Name}' already exists.");
            }
        }

        // Update group details
        group.UpdateDetails(request.Name, request.Description);
        await _groupRepository.UpdateAsync(group);

        // Log the update
        await LogGroupUpdate(group);

        _logger.LogInformation("Group '{GroupName}' (ID: {GroupId}) updated successfully", group.Name, group.Id);

        return new UpdateGroupResult(
            group.Id,
            group.Name,
            group.Description,
            group.LastUpdatedAt ?? DateTime.UtcNow
        );
    }

    private async Task LogGroupUpdate(AuthGroup group)
    {
        try
        {
            var ipAddress = await _serverAddress.GetCurrentIPv4ServerAddress();
            var auditLog = new AuditLog(
                Guid.NewGuid(),
                Guid.Empty,
                AuditLogActions.GroupUpdated,
                $"Group '{group.Name}' updated",
                ipAddress
            );

            await _auditLogRepository.AddAsync(auditLog);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log group update audit");
        }
    }
}
