using AuthService.API.Contracts;
using AuthService.Application.Features.Roles.Commands.AssignRole;
using AuthService.Application.Features.Roles.Commands.CreateRole;
using AuthService.Application.Features.Roles.Commands.DeleteRole;
using AuthService.Application.Features.Roles.Commands.RemoveRole;
using AuthService.Application.Features.Roles.Commands.UpdateRole;
using AuthService.Application.Features.Roles.Queries.GetRoleUsers;
using AuthService.Application.Features.Roles.Queries.GetRoles;
using AuthService.Application.Features.Roles.Queries.GetUserRoles;
using AuthService.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/roles")]
[Authorize] // Require authentication for all endpoints
public class RolesController : ControllerBase
{
    private readonly ILogger<RolesController> _logger;
    private readonly IMediator _mediator;

    public RolesController(ILogger<RolesController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get all roles with pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(RolesListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoles(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var query = new GetRolesQuery(pageNumber, pageSize);
            var result = await _mediator.Send(query);

            var response = new RolesListResponse(
                result.Roles.Select(r => new RoleResponse(
                    r.Id,
                    r.Name,
                    r.Description,
                    r.CreatedAt,
                    r.LastUpdatedAt
                )),
                result.TotalCount,
                result.PageNumber,
                result.PageSize
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
        }
    }

    /// <summary>
    /// Create a new role (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
    [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
    {
        try
        {
            var command = new CreateRoleCommand(request.Name, request.Description);
            var result = await _mediator.Send(command);

            var response = new RoleResponse(
                result.RoleId,
                result.Name,
                result.Description,
                result.CreatedAt,
                null
            );

            return CreatedAtAction(
                nameof(GetRoles),
                new { id = result.RoleId },
                response
            );
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to create role: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
        }
    }

    /// <summary>
    /// Update an existing role (Admin only)
    /// </summary>
    [HttpPut("{roleId:guid}")]
    [Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
    [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateRole(
        Guid roleId,
        [FromBody] UpdateRoleRequest request)
    {
        try
        {
            var command = new UpdateRoleCommand(roleId, request.Name, request.Description);
            var result = await _mediator.Send(command);

            var response = new RoleResponse(
                result.RoleId,
                result.Name,
                result.Description,
                DateTime.UtcNow, // Use current time for CreatedAt (or fetch from DB if needed)
                result.LastUpdatedAt
            );

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to update role: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role {RoleId}", roleId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
        }
    }

    /// <summary>
    /// Delete a role (Admin only)
    /// </summary>
    [HttpDelete("{roleId:guid}")]
    [Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
    [ProducesResponseType(typeof(DeleteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteRole(Guid roleId)
    {
        try
        {
            var command = new DeleteRoleCommand(roleId);
            var result = await _mediator.Send(command);

            var response = new DeleteResponse(result.Success, result.Message);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to delete role: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting role {RoleId}", roleId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
        }
    }

    /// <summary>
    /// Assign a role to a user (Admin only)
    /// </summary>
    [HttpPost("assign")]
    [Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
    [ProducesResponseType(typeof(AssignRoleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
    {
        try
        {
            var command = new AssignRoleCommand(request.UserId, request.RoleId);
            var result = await _mediator.Send(command);

            var response = new AssignRoleResponse(
                result.Success,
                result.Message,
                result.UserId,
                result.RoleId,
                result.RoleName
            );

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to assign role: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
        }
    }

    /// <summary>
    /// Remove a role from a user (Admin only)
    /// </summary>
    [HttpDelete("{roleId:guid}/users/{userId:guid}")]
    [Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
    [ProducesResponseType(typeof(DeleteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RemoveRole(Guid roleId, Guid userId)
    {
        try
        {
            var command = new RemoveRoleCommand(userId, roleId);
            var result = await _mediator.Send(command);

            var response = new DeleteResponse(result.Success, result.Message);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to remove role: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role {RoleId} from user {UserId}", roleId, userId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
        }
    }

    /// <summary>
    /// Get all users assigned to a specific role
    /// </summary>
    [HttpGet("{roleId:guid}/users")]
    [ProducesResponseType(typeof(RoleUsersResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRoleUsers(Guid roleId)
    {
        try
        {
            var query = new GetRoleUsersQuery(roleId);
            var result = await _mediator.Send(query);

            var response = new RoleUsersResponse(
                result.RoleId,
                result.RoleName,
                result.Users.Select(u => new UserSummaryResponse(
                    u.Id,
                    u.UserName,
                    u.Email
                ))
            );

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Role not found: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users for role {RoleId}", roleId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
        }
    }

    /// <summary>
    /// Get all roles assigned to a specific user
    /// </summary>
    [HttpGet("users/{userId:guid}")]
    [ProducesResponseType(typeof(UserRoleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserRoles(Guid userId)
    {
        try
        {
            var query = new GetUserRolesQuery(userId);
            var result = await _mediator.Send(query);

            var response = new UserRoleResponse(
                result.UserId,
                result.UserName,
                result.Roles.Select(r => new RoleResponse(
                    r.Id,
                    r.Name,
                    r.Description,
                    r.CreatedAt,
                    r.LastUpdatedAt
                ))
            );

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "User not found: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles for user {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
        }
    }
}
