using AuthService.Application.Features.Permissions.Commands.AssignPermissionToGroup;
using AuthService.Application.Features.Permissions.Commands.CreatePermission;
using AuthService.Application.Features.Permissions.Commands.RevokePermissionFromGroup;
using AuthService.Application.Features.Permissions.Queries.GetUserPermissions;
using AuthService.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/permissions")]
[Authorize]
public class PermissionsController : ControllerBase
{
    private readonly ILogger<PermissionsController> _logger;
    private readonly IMediator _mediator;

    public PermissionsController(ILogger<PermissionsController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new permission (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
    [ProducesResponseType(typeof(CreatePermissionResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionRequest request)
    {
        try
        {
            var command = new CreatePermissionCommand(request.Key, request.Name, request.Description);
            var result = await _mediator.Send(command);

            return CreatedAtAction(
                nameof(GetUserPermissions),
                new { userId = Guid.Empty },
                result
            );
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to create permission: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating permission");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
        }
    }

    /// <summary>
    /// Assign a permission to a group (Admin only)
    /// </summary>
    [HttpPost("assign")]
    [Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
    [ProducesResponseType(typeof(AssignPermissionToGroupResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AssignPermissionToGroup([FromBody] AssignPermissionRequest request)
    {
        try
        {
            // TODO: Get current user ID from JWT claims
            var currentUserId = Guid.Parse(User.FindFirst("sub")?.Value ?? Guid.Empty.ToString());
            
            var command = new AssignPermissionToGroupCommand(
                request.GroupId,
                request.PermissionId,
                currentUserId);
            
            var result = await _mediator.Send(command);

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to assign permission: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning permission");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
        }
    }

    /// <summary>
    /// Revoke a permission from a group (Admin only)
    /// </summary>
    [HttpDelete("revoke")]
    [Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
    [ProducesResponseType(typeof(RevokePermissionFromGroupResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RevokePermissionFromGroup([FromBody] RevokePermissionRequest request)
    {
        try
        {
            // TODO: Get current user ID from JWT claims
            var currentUserId = Guid.Parse(User.FindFirst("sub")?.Value ?? Guid.Empty.ToString());
            
            var command = new RevokePermissionFromGroupCommand(
                request.GroupId,
                request.PermissionId,
                currentUserId);
            
            var result = await _mediator.Send(command);

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to revoke permission: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking permission");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
        }
    }

    /// <summary>
    /// Get all effective permissions for a user
    /// </summary>
    [HttpGet("users/{userId:guid}")]
    [ProducesResponseType(typeof(GetUserPermissionsResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserPermissions(Guid userId)
    {
        try
        {
            var query = new GetUserPermissionsQuery(userId);
            var result = await _mediator.Send(query);

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "User not found: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving permissions for user {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
        }
    }
}

// Request DTOs
public record CreatePermissionRequest(string Key, string Name, string Description);
public record AssignPermissionRequest(Guid GroupId, Guid PermissionId);
public record RevokePermissionRequest(Guid GroupId, Guid PermissionId);
