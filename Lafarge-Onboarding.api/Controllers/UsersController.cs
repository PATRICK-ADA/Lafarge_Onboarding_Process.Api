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
        return Ok(ApiResponse<PaginatedResponse<GetUserResponse>>.Success(result));
    }



    [HttpPost("bulk-users-upload")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> UploadBulkUsers([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse<object>.Failure("File is required"));
        }

        var allowedExtensions = new[] { ".xlsx", ".xls", ".csv" };
        var fileExtension = Path.GetExtension(file.FileName).ToLower();
        if (!allowedExtensions.Contains(fileExtension))
        {
            return BadRequest(ApiResponse<object>.Failure("Invalid file type. Only Excel (.xlsx, .xls) and CSV files are allowed."));
        }

        return Ok(ApiResponse<string>.Success(await _usersService.UploadBulkUsersAsync(file)));
    }
    


    [HttpGet("get-by-role/{role}")]
    [Authorize(Roles = "HR_ADMIN")]
    public async Task<IActionResult> GetUsersByRole(string role, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var pagination = new PaginationRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _usersService.GetUsersByRoleAsync(role, pagination);
        return Ok(ApiResponse<PaginatedResponse<GetUserResponse>>.Success(result));
    }



    [HttpGet("get-by-name/{name}")]
    [Authorize(Roles = "HR_ADMIN")]
    public async Task<IActionResult> GetUsersByName(string name, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var pagination = new PaginationRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _usersService.GetUsersByNameAsync(name, pagination);
        return Ok(ApiResponse<PaginatedResponse<GetUserResponse>>.Success(result));
    }


    [HttpGet("get-user/{id}")]
    [Authorize(Roles = "HR_ADMIN")]
    public async Task<IActionResult> GetUserById(string id)
    {
        var result = await _usersService.GetUserByIdAsync(id);
        return Ok(ApiResponse<GetUserResponse>.Success(result));
    }




    [HttpPut("update-user/{id}")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> UpdateUserById(string id, [FromBody] UpdateUserRequest request)
    {
        return !ModelState.IsValid
            ? BadRequest(ApiResponse<object>.Failure(string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))))
            : Ok(ApiResponse<string>.Success(await _usersService.UpdateUserByIdAsync(id, request)));
    }



    [HttpPut("update-bulk-users")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> UpdateBulkUsers([FromBody] UpdateBulkUsersRequest request)
    {
       return !ModelState.IsValid
           ? BadRequest(ApiResponse<object>.Failure(string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))))
           : Ok(ApiResponse<string>.Success(await _usersService.UpdateBulkUsersAsync(request)));
    }

    [HttpDelete("delete-user/{id}")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> DeleteUserById(string id)
    {
        await _usersService.DeleteUserByIdAsync(id);
        return Ok(ApiResponse<string>.Success("User deleted successfully"));
    }


    [HttpDelete("delete-bulk-users/{role}")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    public async Task<IActionResult> DeleteBulkUsersByRole(string role)
    {
        var result = await _usersService.DeleteBulkUsersByRoleAsync(role);
        return Ok(ApiResponse<string>.Success(result));
    }
}