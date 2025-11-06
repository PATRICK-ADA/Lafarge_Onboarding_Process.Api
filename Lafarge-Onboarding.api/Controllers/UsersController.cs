namespace Lafarge_Onboarding.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;

    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
    }

    [HttpGet("view-users")]
    [Authorize(Roles = "HR_ADMIN")]
    public async Task<IActionResult> ViewUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var pagination = new PaginationRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _usersService.GetUsersAsync(pagination);
        return Ok(ApiResponse<PaginatedResponse<GetUserResponse>>.Success(result, "Users retrieved successfully"));
    }

    [HttpPost("bulk-upload")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> UploadBulkUsers([FromBody] UploadBulkUsersRequests request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<object>.Failure(string.Join("; ", errors)));
        }

        var result = await _usersService.UploadBulkUsersAsync(request);
        return Ok(result);
    }

    [HttpGet("by-role/{role}")]
    [Authorize(Roles = "HR_ADMIN")]
    public async Task<IActionResult> GetUsersByRole(string role, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var pagination = new PaginationRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _usersService.GetUsersByRoleAsync(role, pagination);
        return Ok(ApiResponse<PaginatedResponse<GetUserResponse>>.Success(result, $"Users with role '{role}' retrieved successfully"));
    }

    [HttpGet("by-name/{name}")]
    [Authorize(Roles = "HR_ADMIN")]
    public async Task<IActionResult> GetUsersByName(string name, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var pagination = new PaginationRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _usersService.GetUsersByNameAsync(name, pagination);
        return Ok(ApiResponse<PaginatedResponse<GetUserResponse>>.Success(result, $"Users with name containing '{name}' retrieved successfully"));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "HR_ADMIN")]
    public async Task<IActionResult> GetUserById(string id)
    {
        var result = await _usersService.GetUserByIdAsync(id);
        if (result.Data == null)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> UpdateUserById(string id, [FromBody] UpdateUserRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<object>.Failure(string.Join("; ", errors)));
        }

        var result = await _usersService.UpdateUserByIdAsync(id, request);
        if (result.StatusCode != "200")
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpPut("bulk/role")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> UpdateBulkUsersByRole([FromBody] UpdateBulkUsersRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<object>.Failure(string.Join("; ", errors)));
        }

        var result = await _usersService.UpdateBulkUsersByRoleAsync(request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> DeleteUserById(string id)
    {
        var result = await _usersService.DeleteUserByIdAsync(id);
        if (result.StatusCode != "200")
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpDelete("bulk/role/{role}")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    public async Task<IActionResult> DeleteBulkUsersByRole(string role)
    {
        var result = await _usersService.DeleteBulkUsersByRoleAsync(role);
        return Ok(result);
    }
}