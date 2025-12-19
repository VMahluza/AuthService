using AuthService.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.Roles.Queries.GetRoles;

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, GetRolesResult>
{
    private readonly ILogger<GetRolesQueryHandler> _logger;
    private readonly IRoleRepository _roleRepository;

    public GetRolesQueryHandler(
        ILogger<GetRolesQueryHandler> logger,
        IRoleRepository roleRepository)
    {
        _logger = logger;
        _roleRepository = roleRepository;
    }

    public async Task<GetRolesResult> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        // Get paginated roles
        var roles = await _roleRepository.GetAllAsync(request.PageNumber, request.PageSize);
        
        // Get total count (for now, we'll use the page count; in production you'd have a separate count method)
        var rolesList = roles.ToList();
        
        // Map to DTOs
        var roleDtos = rolesList.Select(role => new RoleDto(
            role.Id,
            role.Name,
            role.Description,
            role.CreatedAt,
            role.LastUpdatedAt
        ));

        _logger.LogInformation(
            "Retrieved {Count} roles for page {PageNumber} with page size {PageSize}",
            rolesList.Count, request.PageNumber, request.PageSize);

        return new GetRolesResult(
            roleDtos,
            rolesList.Count, // In production, this should be total count from DB
            request.PageNumber,
            request.PageSize
        );
    }
}
