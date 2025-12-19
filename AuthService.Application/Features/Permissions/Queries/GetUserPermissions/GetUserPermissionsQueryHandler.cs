using AuthService.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.Permissions.Queries.GetUserPermissions;

public class GetUserPermissionsQueryHandler : IRequestHandler<GetUserPermissionsQuery, GetUserPermissionsResult>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserPermissionsQueryHandler> _logger;

    public GetUserPermissionsQueryHandler(
        IPermissionRepository permissionRepository,
        IUserRepository userRepository,
        ILogger<GetUserPermissionsQueryHandler> logger)
    {
        _permissionRepository = permissionRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<GetUserPermissionsResult> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID '{request.UserId}' not found.");
        }

        var permissions = await _permissionRepository.GetEffectivePermissionsByUserIdAsync(request.UserId);

        var permissionDTOs = permissions.Select(p => new PermissionDTO(
            p.Id,
            p.Key,
            p.Name,
            p.Description
        ));

        return new GetUserPermissionsResult(
            user.Id,
            user.UserName,
            permissionDTOs
        );
    }
}
