namespace Lafarge_Onboarding.api.Controllers;

using Lafarge_Onboarding.domain.Dtos.OnboardingResponses;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ContentController : ControllerBase
{
    private readonly ILocalHireInfoService _localHireInfoService;
    private readonly ILogger<ContentController> _logger;

    public ContentController(ILocalHireInfoService localHireInfoService, ILogger<ContentController> logger)
    {
        _localHireInfoService = localHireInfoService;
        _logger = logger;
    }

    [HttpPost("local-hire-info-upload")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<LocalHireInfoResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> UploadLocalHireInfo([FromForm] IFormFile file)
    {
        _logger.LogInformation("Local hire info upload request received");

        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse<object>.Failure("File is required"));
        }

        try
        {
            var result = await _localHireInfoService.ExtractAndSaveLocalHireInfoAsync(file);
            return Ok(ApiResponse<LocalHireInfoResponse>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing local hire info upload");
            return BadRequest(ApiResponse<object>.Failure("Error processing file"));
        }
    }

    [HttpGet("get-local-hire-info")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<LocalHireInfoResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetLocalHireInfo()
    {
        _logger.LogInformation("Get local hire info request received");

        try
        {
            var result = await _localHireInfoService.GetLocalHireInfoAsync();
            if (result == null)
            {
                return NotFound(ApiResponse<object>.Failure("Local hire info not found"));
            }

            return Ok(ApiResponse<LocalHireInfoResponse>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving local hire info");
            return BadRequest(ApiResponse<object>.Failure("Error retrieving data"));
        }
    }
}