using MediatR;

namespace AuthService.Application.Features.Groups.Commands.RemoveUserFromGroup;

public record RemoveUserFromGroupCommand(
    Guid UserId,
    Guid GroupId
) : IRequest<RemoveUserFromGroupResult>;
