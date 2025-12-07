using AuthService.Domain.Constants;
using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Entities.User;
using AuthService.Domain.Interfaces;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Domain.Interfaces.Services;
using AuthService.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Features.Auth.Commands.Register;
public class RegisterUserCommandHandler : 
    IRequestHandler<RegisterUserCommand, RegisterUserResult>
{

    ILogger<RegisterUserCommandHandler> _logger;

    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IBase64TokenGenerator _tokenGenerator;
    private readonly IEmailVerificationTokenRepository _emailVerificationTokenRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IAuthEmailSender _authEmailSender;
    private readonly IServerAddress _serverAddress;

    public RegisterUserCommandHandler(
        ILogger<RegisterUserCommandHandler> logger,
        IUserRepository userRepository, 
        IPasswordHasher passwordHasher,
        IBase64TokenGenerator tokenGenerator,
        IEmailVerificationTokenRepository emailVerificationTokenRepository,
        IAuditLogRepository auditLogRepository,
        IAuthEmailSender authEmailSender,
        IServerAddress currentServerAddress)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _emailVerificationTokenRepository = emailVerificationTokenRepository;
        _auditLogRepository = auditLogRepository;
        _authEmailSender = authEmailSender;
        _logger = logger;
        _serverAddress = currentServerAddress;
    }

    public async Task<RegisterUserResult> 
        Handle(RegisterUserCommand request, 
        CancellationToken cancellationToken)
    {

        await CheckIfUserExistsByEmailOrUsername(request);

        PasswordHash passwordHash = _passwordHasher.Hash(request.Password);
        EmailAddress emailAddress = EmailAddress.Create(request.Email);

        User newUser = await AddUserToDb(request, passwordHash);

        await AddEmailVarificationTokenToDb(newUser);
        await LogEvent(request, newUser);

        return new RegisterUserResult(
            UserId: newUser.Id,
            UserName: newUser.UserName,
            Email: newUser.Email.Value
        );
    }

    private async Task LogEvent(RegisterUserCommand request, User newUser)
    {
        var ipAddress = await _serverAddress.GetCurrentIPv4ServerAddress();

        var auditLog = new AuditLog(
            Guid.NewGuid(),
            newUser.Id,
            AuditLogActions.UserRegistered,
            $"New user registered with email {newUser.Email.Value}",
            ipAddress
        );
        await _auditLogRepository.AddAsync(auditLog);
    }

    private async Task AddEmailVarificationTokenToDb(User newUser)
    {
        string token = await _tokenGenerator.GenerateToken();
        var emailVerificationToken = new EmailVerificationToken(
            Guid.NewGuid(),
            newUser.Id,
            token,
            DateTime.UtcNow.AddHours(24),
            null
        );
     
        await _emailVerificationTokenRepository.AddAsync(emailVerificationToken);
        await _authEmailSender.SendVarificationEmailAsync(newUser, token);
    }

    private async Task<User> AddUserToDb(RegisterUserCommand request, PasswordHash passwordHash)
    {
        User newUser = User.RegisterNew(
            request.UserName,
            request.Email,
            passwordHash.Value);

        await _userRepository.AddAsync(newUser);
        return newUser;
    }

    private async Task CheckIfUserExistsByEmailOrUsername(RegisterUserCommand request)
    {
        User existingUserByEmail = await _userRepository
            .GetByEmailAsync(request.Email);
        if (existingUserByEmail is not null)
        {
            throw new InvalidOperationException(
                $"Email {request.Email} is already in use."
                );
        }

        var existingUserByUsername = await _userRepository
            .GetByUsernameAsync(request.UserName);
        if (existingUserByUsername is not null)
        {
            throw new InvalidOperationException(
                $"Username {request.UserName} is already in use."
                );
        }
    }
}