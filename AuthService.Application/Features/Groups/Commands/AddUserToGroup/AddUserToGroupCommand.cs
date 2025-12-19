using MediatR;

namespace AuthService.Application.Features.Groups.Commands.AddUserToGroup;

public record AddUserToGroupCommand(
    Guid UserId,
    Guid GroupId
) : IRequest<AddUserToGroupResult>;
