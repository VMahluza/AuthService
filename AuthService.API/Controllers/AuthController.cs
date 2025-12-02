using AuthService.API.Contracts;
using AuthService.Application.Features.Auth.Commands.Login;
using AuthService.Application.Features.Auth.Commands.Register;
using AuthService.Application.Features.Auth.Commands.VerifyEmail;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace AuthService.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IMediator _mediator;

    public AuthController(ILogger<AuthController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        RegisterUserCommand command = new RegisterUserCommand(
            UserName: request.UserName,
            Email: request.Email,
            Password: request.Password
        );

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        try
        {
            VerifyEmailCommand command = new VerifyEmailCommand(
                token: token
            );
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(
                "Invalid email verification attempt with token {Token}: {Message}",
                token, ex.Message);

            return BadRequest(
                new VerifyEmailResponse
                (
                    false,
                   "Email verification failed: " + ex.Message
                )
            );
        }
        catch (NullReferenceException ex)
        {
            _logger.LogWarning(
                "Email verification attempt with token {Token} failed: {Message}",
                token, ex.Message);
            return BadRequest(
                new VerifyEmailResponse
                (
                    false,
                   "Email verification failed: " + ex.Message
                ));
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new VerifyEmailResponse(false, "Internal Server Error"));
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {

        try
        {
            LoginUserCommand command = new LoginUserCommand(
                UserName: request.UserName,
                Password: request.Password
            );

            var result = await _mediator.Send(command);
            return Ok(result);

        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(
                "Unauthorized login attempt for user {UserName}", 
                request.UserName);
            return Unauthorized();
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError, 
                "Internal Server Error");
        }
    }
}
