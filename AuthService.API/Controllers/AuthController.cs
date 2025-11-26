using AuthService.API.Contracts;
using AuthService.Application.Features.Auth.Commands.Login;
using AuthService.Application.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

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
