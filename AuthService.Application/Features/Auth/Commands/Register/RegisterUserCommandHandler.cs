using AuthService.Domain.Interfaces;
using AuthService.Domain.Interfaces.Repositories;
using MediatR;

namespace AuthService.Application.Features.Auth.Commands.Register;
public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{

    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {

        // TODO: 1. Validation: Check if the email is already taken
        // TODO: 2. Validation: Check if the username is already taken
        // TODO: 3. Security: Hash the password (Infrastructure Responsibility)
        // TODO: 4. Domain Logic: Create the User Aggregate (Domain Responsibility)
        // TODO: 5. Persistence: Save the new user (Infrastructure Responsibility)
        // TODO: 6. Output: Return the result DTO
        throw new NotImplementedException();
    }
}