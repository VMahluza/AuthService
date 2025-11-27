using MediatR;
namespace AuthService.Application.Features.Auth.Commands.VarifyEmail;

public record VarifyEmailCommand(
    string token
    ) : IRequest<VarifyEmailResult> ;

