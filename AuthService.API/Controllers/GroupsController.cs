using AuthService.Application.Features.Groups.Commands.AddUserToGroup;
using AuthService.Application.Features.Groups.Commands.CreateGroup;
using AuthService.Application.Features.Groups.Commands.DeleteGroup;
using AuthService.Application.Features.Groups.Commands.RemoveUserFromGroup;
using AuthService.Application.Features.Groups.Commands.UpdateGroup;
using AuthService.Application.Features.Groups.Queries.GetGroupUsers;
using AuthService.Application.Features.Groups.Queries.GetGroups;
using AuthService.Application.Features.Groups.Queries.GetUserGroups;
using AuthService.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthService.API.Contracts.Common;
using AuthService.API.Contracts.Groups.AddUserToGroup;
using AuthService.API.Contracts.Groups.CreateGroup;
using AuthService.API.Contracts.Groups.UpdateGroup;
using AuthService.API.Contracts.Groups.GetGroup;
using AuthService.API.Contracts.Groups.Common;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/groups")]
[Authorize] // Require authentication for all endpoints
public class GroupsController : ControllerBase
{
    private readonly ILogger<GroupsController> _logger;
    private readonly IMediator _mediator;

    public GroupsController(ILogger<GroupsController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Get all groups with pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(GroupsListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGroups(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var query = new GetGroupsQuery(pageNumber, pageSize);
            var result = await _mediator.Send(query);

            var response = new GroupsListResponse(
                result.Groups.Select(g => new GroupResponse(
                    g.Id,
                    g.Name,
                    g.Description,
                    g.CreatedAt,
                    g.LastUpdatedAt
                )),
                result.TotalCount,
                result.PageNumber,
                result.PageSize
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving groups");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
        }
    }

    /// <summary>
    /// Create a new group (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
    [ProducesResponseType(typeof(GroupResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request)
    {
        try
        {
            var command = new CreateGroupCommand(request.Name, request.Description);
            var result = await _mediator.Send(command);

            var response = new GroupResponse(
                result.GroupId,
                result.Name,
                result.Description,
                result.CreatedAt,
                null
            );

            return CreatedAtAction(
                nameof(GetGroups),
                new { id = result.GroupId },
                response
            );
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to create group: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating group");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
        }
    }

    /// <summary>
    /// Update an existing group (Admin only)
    /// </summary>
    [HttpPut("{groupId:guid}")]
    [Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
    [ProducesResponseType(typeof(GroupResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateGroup(
        Guid groupId,
        [FromBody] UpdateGroupRequest request)
    {
        try
        {
            var command = new UpdateGroupCommand(groupId, request.Name, request.Description);
            var result = await _mediator.Send(command);

            var response = new GroupResponse(
                result.GroupId,
                result.Name,
                result.Description,
                DateTime.UtcNow, // Use current time for CreatedAt (or fetch from DB if needed)
                result.LastUpdatedAt
            );

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to update group: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating group {GroupId}", groupId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
        }
    }

    /// <summary>
    /// Delete a group (Admin only)
    /// </summary>
    [HttpDelete("{groupId:guid}")]
    [Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
    [ProducesResponseType(typeof(DeleteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteGroup(Guid groupId)
    {
        try
        {
            var command = new DeleteGroupCommand(groupId);
            var result = await _mediator.Send(command);

            var response = new DeleteResponse(result.Success, result.Message);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to delete group: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting group {GroupId}", groupId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
        }
    }

    /// <summary>
    /// Add a user to a group (Admin only)
    /// </summary>
    [HttpPost("members")]
    [Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
    [ProducesResponseType(typeof(AddUserToGroupResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AddUserToGroup([FromBody] AddUserToGroupRequest request)
    {
        try
        {
            var command = new AddUserToGroupCommand(request.UserId, request.GroupId);
            var result = await _mediator.Send(command);

            var response = new AddUserToGroupResponse(
                result.Success,
                result.Message,
                result.UserId,
                result.GroupId,
                result.GroupName
            );

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to add user to group: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user to group");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
        }
    }

    /// <summary>
    /// Remove a user from a group (Admin only)
    /// </summary>
    [HttpDelete("{groupId:guid}/users/{userId:guid}")]
    [Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
    [ProducesResponseType(typeof(DeleteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RemoveUserFromGroup(Guid groupId, Guid userId)
    {
        try
        {
            var command = new RemoveUserFromGroupCommand(userId, groupId);
            var result = await _mediator.Send(command);

            var response = new DeleteResponse(result.Success, result.Message);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to remove user from group: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing user {UserId} from group {GroupId}", userId, groupId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
        }
    }

    /// <summary>
    /// Get all users in a specific group
    /// </summary>
    [HttpGet("{groupId:guid}/users")]
    [ProducesResponseType(typeof(GroupUsersResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGroupUsers(Guid groupId)
    {
        try
        {
            var query = new GetGroupUsersQuery(groupId);
            var result = await _mediator.Send(query);

            var response = new GroupUsersResponse(
                result.GroupId,
                result.GroupName,
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
            _logger.LogWarning(ex, "Group not found: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users for group {GroupId}", groupId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
        }
    }

    /// <summary>
    /// Get all groups a specific user belongs to
    /// </summary>
    [HttpGet("users/{userId:guid}")]
    [ProducesResponseType(typeof(UserGroupResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserGroups(Guid userId)
    {
        try
        {
            var query = new GetUserGroupsQuery(userId);
            var result = await _mediator.Send(query);

            var response = new UserGroupResponse(
                result.UserId,
                result.UserName,
                result.Groups.Select(g => new GroupResponse(
                    g.Id,
                    g.Name,
                    g.Description,
                    g.CreatedAt,
                    g.LastUpdatedAt
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
            _logger.LogError(ex, "Error retrieving groups for user {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error");
        }
    }
}
