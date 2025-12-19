using MediatR;

namespace AuthService.Application.Features.Groups.Commands.CreateGroup;

public record CreateGroupCommand(
    string Name,
    string Description
) : IRequest<CreateGroupResult>;
