using AuthService.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.Roles.Queries.GetUserRoles;

public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, GetUserRolesResult>
{
    private readonly ILogger<GetUserRolesQueryHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;

    public GetUserRolesQueryHandler(
        ILogger<GetUserRolesQueryHandler> logger,
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
    }

    public async Task<GetUserRolesResult> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        // Verify user exists
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID '{request.UserId}' not found.");
        }

        // Get user's roles
        var roles = await _userRoleRepository.GetRolesByUserIdAsync(request.UserId);
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
            "Retrieved {Count} roles for user '{UserName}' (ID: {UserId})",
            rolesList.Count, user.UserName, user.Id);

        return new GetUserRolesResult(
            user.Id,
            user.UserName,
            roleDtos
        );
    }
}
