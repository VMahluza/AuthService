using MediatR;

namespace AuthService.Application.Features.Groups.Commands.DeleteGroup;

public record DeleteGroupCommand(
    Guid GroupId
) : IRequest<DeleteGroupResult>;
