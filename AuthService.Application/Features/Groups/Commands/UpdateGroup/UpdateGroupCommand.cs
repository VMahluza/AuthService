using MediatR;

namespace AuthService.Application.Features.Groups.Commands.UpdateGroup;

public record UpdateGroupCommand(
    Guid GroupId,
    string Name,
    string Description
) : IRequest<UpdateGroupResult>;
