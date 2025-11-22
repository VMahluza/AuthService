using AuthService.Domain.Entities.User;
using AuthService.Domain.Interfaces;
using AuthService.Domain.Interfaces.Repositories;
using MediatR;

namespace AuthService.Application.Features.Auth.Commands.Register;
public class RegisterUserCommandHandler : 
    IRequestHandler<RegisterUserCommand, RegisterUserResult>
{

    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(
        IUserRepository userRepository, 
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterUserResult> 
        Handle(RegisterUserCommand request, 
        CancellationToken cancellationToken)
    {

        Task<User> existingUserByEmail = _userRepository
            .GetByEmailAsync(request.Email);
        if (existingUserByEmail is not null)
        {
            throw new InvalidOperationException(
                $"Email {request.Email} is already in use."
                );
        }

        var existingUserByUsername = _userRepository
            .GetByUsernameAsync(request.UserName);
        if (existingUserByUsername is not null)
        {
            throw new InvalidOperationException(
                $"Username {request.UserName} is already in use."
                );
        }

        PasswordHash passwordHash = PasswordHash.Create(request.Password);


        EmailAddress emailAddress = EmailAddress.Create(request.Email);
        User newUser = User.RegisterNew(
            request.UserName, request.Email, request.Password, _passwordHasher);

        await _userRepository.AddAsync(newUser);

        return new RegisterUserResult(
            UserId: newUser.Id,
            UserName: newUser.UserName,
            Email: newUser.Email.Value
        );
    }
}