using AuthService.API.Contracts;
using AuthService.Application.Features.Auth.Commands.ForgotPassword;
using AuthService.Application.Features.Auth.Commands.Login;
using AuthService.Application.Features.Auth.Commands.Logout;
using AuthService.Application.Features.Auth.Commands.RefreshToken;
using AuthService.Application.Features.Auth.Commands.Register;
using AuthService.Application.Features.Auth.Commands.ResetPassword;
using AuthService.Application.Features.Auth.Commands.VerifyEmail;
using AuthService.Application.Features.Auth.Queries.GetAuditLogsByUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
                new VerifyEmailResponse(false, $"Internal Server Error :{ex.Message}"));
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
            var result = new { 
                Message = $"Unauthorized login attempt for user {request.UserName}: {ex.Message}"
            };
            _logger.LogWarning(
                "Unauthorized login attempt for user {UserName}: {Message}",
                request.UserName, ex.Message);
            return Unauthorized(result);
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError, 
                "Internal Server Error");
        }
    }
    [Authorize]
    [HttpGet("audit-logs/{userId:guid}")]
    public async Task<IActionResult> GetAuditLogsByUser(
        Guid userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            GetAuditLogsByUserQuery query = new GetAuditLogsByUserQuery(
                UserId: userId,
                PageNumber: pageNumber,
                PageSize: pageSize
            );

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving audit logs for user {UserId}", userId);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                "Internal Server Error");
        }
    }

    [HttpDelete("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutUserRequest request)
    {
        try
        {
            var command = new LogoutUserCommand(
                  JwtToken: request.JwtToken,
                  RevokeAllSessions: request.RevokeAllSessions
              );

            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            var result = new LogoutUserResponse(
                false,
                $"Failed to Logout:{ex.Message}"
                );
            return BadRequest(result);
        }
  
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var command = new ForgotPasswordCommand(request.Email);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        try
        {
            var command = new ResetPasswordCommand(
                Token: request.Token,
                NewPassword: request.NewPassword
            );

            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Password reset failed: {Message}", ex.Message);
            return BadRequest(new ResetPasswordResult(false, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new ResetPasswordResult(false, "An error occurred while resetting your password.")
            );
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var command = new RefreshTokenCommand(request.RefreshToken);
            var result = await _mediator.Send(command);
            
            return Ok(new RefreshTokenResponse(
                result.AccessToken,
                result.RefreshToken,
                result.ExpiresAt));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Token refresh failed: {Message}", ex.Message);
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { error = "An error occurred while refreshing the token" });
        }
    }

}
