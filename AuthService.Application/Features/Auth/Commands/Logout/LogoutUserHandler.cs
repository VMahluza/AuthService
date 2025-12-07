using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
namespace AuthService.Application.Features.Auth.Commands.Logout;

public class LogoutUserHandler : IRequestHandler<LogoutUserCommand, LogoutUserResult>
{
    private readonly ILogger<LogoutUserHandler> _logger;
    private readonly IUserSessionRepository _userSessionRepository;

    public LogoutUserHandler(
        ILogger<LogoutUserHandler> logger, 
        IUserSessionRepository userSessionRepository
        )
    {
        _logger = logger;
        _userSessionRepository = userSessionRepository;
    }
    public async Task<LogoutUserResult> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        var userSession = await GetActiveSession(request.JwtToken);
        return 
            request.RevokeAllSessions ? await RemoveAllUserSessions(userSession.UserId) : 
            await RemoveSession(userSession)
            ;
    }

    private async Task<LogoutUserResult> RemoveSession(UserSession userSession)
    {
        await _userSessionRepository.DeleteAsync(userSession);
        return new LogoutUserResult(true, "Logout Successful!");
    }

    private async Task<LogoutUserResult> RemoveAllUserSessions(Guid userId)
    {
        await _userSessionRepository.DeleteAsync(userId);
        return new LogoutUserResult(true, "Logout Successful from all devices or services");
    }

    private async Task<UserSession> GetActiveSession(string jwtToken)
    { 
        var userSession = await _userSessionRepository.GetActiveSessionByTokenAsync(jwtToken);
        if (userSession is null)
            {
            _logger.LogWarning("No active session found for token: {JwtToken}", jwtToken);
            throw new InvalidOperationException("No active session found for the provided token.");
        }
        return userSession;
    }
}
