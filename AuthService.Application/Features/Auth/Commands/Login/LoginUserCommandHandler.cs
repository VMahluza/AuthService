using AuthService.Domain.Constants;
using AuthService.Domain.DTOs;
using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Entities.User;
using AuthService.Domain.Enums;
using AuthService.Domain.Interfaces;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Domain.Interfaces.Services;
using AuthService.Domain.Options;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuthService.Application.Features.Auth.Commands.Login;

/// <summary>
/// why don’t you use abp? comes pre built with user management, auth and multitenancy
/// FastEndpoints
/// </summary>

public class LoginUserCommandHandler : 
    IRequestHandler<LoginUserCommand, LoginUserResult>
{

    private readonly IUserRepository _userRepository;
    private readonly IUserSessionRepository _userSessionRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly SecuritySettingsOptions _securitySettings;
    private readonly ILogger<LoginUserCommandHandler> _logger;
    private readonly IServerAddress _serverAddress;

    public LoginUserCommandHandler(
        IUserRepository userRepository, 
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IUserSessionRepository userSessionRepository,
        IAuditLogRepository auditLogRepository,
        IOptions<SecuritySettingsOptions> securitySettingsOptions,
        ILogger<LoginUserCommandHandler> logger,
        IServerAddress serverAddress
        )
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _userSessionRepository = userSessionRepository;
        _auditLogRepository = auditLogRepository;
        _securitySettings = securitySettingsOptions.Value;
        _logger = logger;
        _serverAddress = serverAddress;
    }

    public async Task<LoginUserResult> Handle(
        LoginUserCommand request, 
        CancellationToken cancellationToken)
    {
        User user = await GetUserForLoginAsync(request);
        await DoAccountStatusChecks(user);
        await VarifyPassword(request, user);
        AuthenticationResult token = await CreateSession(user);


        var auditLog = AuditLog.Create(
            user.Id,
            AuditLogActions.LoginSuccess,
            $"User {user.UserName} logged in successfully.",
            await _serverAddress.GetCurrentIPv4ServerAddress()
            );

        await _auditLogRepository.AddAsync(auditLog);

        return new LoginUserResult(
            user.Id,
            user.UserName,
            user.Email.Value,
            token.AccessToken
            );
    }

    private async Task<AuthenticationResult> CreateSession(User user)
    {
        await EnforceConcurrentSessionPolicyAsync(user.Id);
        AuthenticationResult token = await _jwtTokenGenerator.GenerateToken(user.Id, user.UserName, user.Email.Value);
        var userSession = UserSession.Create(user.Id, token, token.ExpiresAt);

        await _userSessionRepository.AddAsync(userSession);
        return token;
    }

    private async Task<User> GetUserForLoginAsync(LoginUserCommand request)
    {
        User user = await _userRepository.GetByUsernameAsync(request.UserName);
        if (user == null)
        {
            // Failed Authentication Handling:
            throw new UnauthorizedAccessException("Invalid username or password.");
        }
        // Credential Authentication:
        return user;
    }

    private async Task VarifyPassword(LoginUserCommand request, User? user)
    {
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
     
            user.UpdateLoginAttempts(false);
           
            if (user.FailedLoginAttempts + 1 >= _securitySettings.MaxFailedAccessAttempts)
            {
                user.LockAccount();
                await _userRepository.UpdateAsync(user);
                // TODO : Notify user of account lockout via email/SMS
                throw new UnauthorizedAccessException($"Max failed login attempts reached. Account locked. wait for {_securitySettings.DefaultLockoutTimeSpanInMinutes} Minutes and Try again");
            }
            await _userRepository.UpdateAsync(user);
            throw new UnauthorizedAccessException("Invalid username or password.");
        }
    }

    private static async Task DoAccountStatusChecks(User? user)
    {

        if (user == null) {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }
       
        switch (user.Status)
        {
            case UserStatus.Locked:
                throw new UnauthorizedAccessException("The Account is locked. please contact support");
            case UserStatus.Inactive:
                throw new UnauthorizedAccessException("The Account is inactive. please activate your account");
            case UserStatus.Suspended:
                throw new UnauthorizedAccessException("The Account is suspended. please contact support");
            case UserStatus.PendingVerification:
                throw new UnauthorizedAccessException("The Account is pending verification. please verify your account");
            default:
                break;
        }
       
    }

    /// <summary>
    /// Enforces the concurrent session policy based on configuration
    /// </summary>
    private async Task EnforceConcurrentSessionPolicyAsync(Guid userId)
    {

        if (_securitySettings.MaxConcurrentSessions <= 0)
        {
            _logger.LogDebug("Unlimited concurrent sessions allowed for user: {UserId}", userId);
            return;
        }

        var activeSessions = (await _userSessionRepository.GetActiveSessionsByUserIdAsync(userId)).ToList();

        _logger.LogDebug(
            "User {UserId} has {ActiveSessionCount} active sessions. Max allowed: {MaxSessions}",
            userId,
            activeSessions.Count,
            _securitySettings.MaxConcurrentSessions);

        // Check if we're at or over the limit
        if (activeSessions.Count >= _securitySettings.MaxConcurrentSessions)
        {
            switch (_securitySettings.SessionEnforcement)
            {
                case SessionEnforcementStrategy.DenyNew:
                    _logger.LogWarning(
                        "Login denied for user {UserId}. Maximum concurrent sessions ({MaxSessions}) reached.",
                        userId,
                        _securitySettings.MaxConcurrentSessions);

                    throw new UnauthorizedAccessException(
                        $"Maximum concurrent sessions ({_securitySettings.MaxConcurrentSessions}) reached. " +
                        "Please log out from another device or wait for a session to expire.");

                case SessionEnforcementStrategy.RevokeOldest:
                    // Calculate how many sessions need to be revoked
                    int sessionsToRevoke = activeSessions.Count - _securitySettings.MaxConcurrentSessions + 1;
                    var sessionsToRevoke_List = activeSessions
                        .OrderBy(s => s.IssuedAt)
                        .Take(sessionsToRevoke)
                        .ToList();

                    foreach (var session in sessionsToRevoke_List)
                    {
                        session.Revoke();
                        await _userSessionRepository.UpdateAsync(session);

                        _logger.LogInformation(
                            "Revoked oldest session {SessionId} for user {UserId} due to concurrent session limit",
                            session.Id,
                            userId);
                    }
                    break;

                case SessionEnforcementStrategy.Unlimited:
                default:
                    break;
            }
        }
    }
}
