using AuthService.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.Roles.Queries.GetRoleUsers;

public class GetRoleUsersQueryHandler : IRequestHandler<GetRoleUsersQuery, GetRoleUsersResult>
{
    private readonly ILogger<GetRoleUsersQueryHandler> _logger;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;

    public GetRoleUsersQueryHandler(
        ILogger<GetRoleUsersQueryHandler> logger,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository)
    {
        _logger = logger;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
    }

    public async Task<GetRoleUsersResult> Handle(GetRoleUsersQuery request, CancellationToken cancellationToken)
    {
        // Verify role exists
        var role = await _roleRepository.GetByIdAsync(request.RoleId);
        if (role == null)
        {
            throw new InvalidOperationException($"Role with ID '{request.RoleId}' not found.");
        }

        // Get users with this role
        var users = await _userRoleRepository.GetUsersByRoleIdAsync(request.RoleId);
        var usersList = users.ToList();

        // Map to DTOs
        var userDtos = usersList.Select(user => new UserDto(
            user.Id,
            user.UserName,
            user.Email.Value
        ));

        _logger.LogInformation(
            "Retrieved {Count} users for role '{RoleName}' (ID: {RoleId})",
            usersList.Count, role.Name, role.Id);

        return new GetRoleUsersResult(
            role.Id,
            role.Name,
            userDtos
        );
    }
}
