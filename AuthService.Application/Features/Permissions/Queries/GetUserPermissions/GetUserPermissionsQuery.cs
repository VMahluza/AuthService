using MediatR;

namespace AuthService.Application.Features.Permissions.Queries.GetUserPermissions;

public record GetUserPermissionsQuery(Guid UserId) : IRequest<GetUserPermissionsResult>;
