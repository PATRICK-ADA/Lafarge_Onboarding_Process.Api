using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Lafarge_Onboarding.application.Abstraction;
using Lafarge_Onboarding.domain.Entities;
using Lafarge_Onboarding.domain.OnboardingRequests;
using Lafarge_Onboarding.domain.OnboardingResponses;

namespace Lafarge_Onboarding.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
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
    public async Task<IActionResult> UploadBulkUsers([FromBody] UploadBulkUsersRequests request)
    {
        if (request == null || !request.Users.Any())
        {
            return BadRequest(ApiResponse<string>.Failure("Request cannot be null or empty"));
        }

        var result = await _usersService.UploadBulkUsersAsync(request);
        return Ok(result);
    }
}