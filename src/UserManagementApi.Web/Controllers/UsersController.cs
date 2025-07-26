using Microsoft.AspNetCore.Mvc;
using UserManagementApi.Core.Interfaces;
using UserManagementApi.Web.Models;

namespace UserManagementApi.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get all users
    /// </summary>
    /// <returns>List of all users</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAllUsers()
    {
        _logger.LogInformation("Getting all users");
        
        var users = await _userService.GetAllUsersAsync();
        var response = users.Select(u => new UserResponse
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt
        });

        return Ok(response);
    }

    /// <summary>
    /// Get a user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>The user if found</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserResponse>> GetUser(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Invalid user ID provided: {Id}", id);
            return BadRequest("Invalid user ID");
        }

        _logger.LogInformation("Getting user with ID: {Id}", id);

        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            _logger.LogWarning("User not found with ID: {Id}", id);
            return NotFound($"User with ID {id} not found");
        }

        var response = new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

        return Ok(response);
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="request">User creation data</param>
    /// <returns>The created user</returns>
    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserResponse>> CreateUser([FromBody] CreateUserRequest request)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for user creation: {@Request}", request);
            return BadRequest(ModelState);
        }

        try
        {
            _logger.LogInformation("Creating new user: {Name} - {Email}", request.Name, request.Email);

            var user = await _userService.CreateUserAsync(request.Name, request.Email);
            var response = new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            _logger.LogInformation("User created successfully with ID: {Id}", user.Id);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid argument for user creation: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return StatusCode(500, "An error occurred while creating the user");
        }
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">User update data</param>
    /// <returns>The updated user</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserResponse>> UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Invalid user ID provided: {Id}", id);
            return BadRequest("Invalid user ID");
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for user update: {@Request}", request);
            return BadRequest(ModelState);
        }

        try
        {
            _logger.LogInformation("Updating user with ID: {Id}", id);

            var user = await _userService.UpdateUserAsync(id, request.Name, request.Email);
            if (user == null)
            {
                _logger.LogWarning("User not found for update with ID: {Id}", id);
                return NotFound($"User with ID {id} not found");
            }

            var response = new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            _logger.LogInformation("User updated successfully with ID: {Id}", user.Id);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid argument for user update: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID: {Id}", id);
            return StatusCode(500, "An error occurred while updating the user");
        }
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteUser(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning("Invalid user ID provided: {Id}", id);
            return BadRequest("Invalid user ID");
        }

        _logger.LogInformation("Deleting user with ID: {Id}", id);

        var deleted = await _userService.DeleteUserAsync(id);
        if (!deleted)
        {
            _logger.LogWarning("User not found for deletion with ID: {Id}", id);
            return NotFound($"User with ID {id} not found");
        }

        _logger.LogInformation("User deleted successfully with ID: {Id}", id);
        return NoContent();
    }
}
