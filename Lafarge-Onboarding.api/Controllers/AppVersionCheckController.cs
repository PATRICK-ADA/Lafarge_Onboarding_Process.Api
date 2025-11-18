namespace Lafarge_Onboarding.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class AppVersionCheckController : ControllerBase
{
    private readonly IAppVersionService _appVersionService;
    private readonly ILogger<AppVersionCheckController> _logger;

    public AppVersionCheckController(IAppVersionService appVersionService, ILogger<AppVersionCheckController> logger)
    {
        _appVersionService = appVersionService;
        _logger = logger;
    }

    [HttpPost("create")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<AppVersionResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> CreateAppVersion([FromBody] AppVersionRequest request)
    {
        return !ModelState.IsValid
            ? BadRequest(ApiResponse<object>.Failure(string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))))
            : Ok(ApiResponse<AppVersionResponse>.Success(await _appVersionService.CreateAppVersionAsync(request), "App version created successfully"));
    }

    [HttpGet("latest")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<AppVersionResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetLatestAppVersion([FromQuery] string appName)
    {
        if (string.IsNullOrEmpty(appName))
        {
            return BadRequest(ApiResponse<object>.Failure("AppName is required"));
        }

        var result = await _appVersionService.GetLatestAppVersionAsync(appName);
        return result == null
            ? NotFound(ApiResponse<object>.Failure("No app version found", "404"))
            : Ok(ApiResponse<AppVersionResponse>.Success(result, "Latest AppUpDate fetched successfully"));
    }
}