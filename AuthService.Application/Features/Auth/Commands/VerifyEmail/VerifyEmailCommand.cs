using MediatR;
namespace AuthService.Application.Features.Auth.Commands.VerifyEmail;

public record VerifyEmailCommand(
    string token
    ) : IRequest<VerifyEmailResult> ;

