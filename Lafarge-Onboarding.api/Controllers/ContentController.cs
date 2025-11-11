namespace Lafarge_Onboarding.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ContentController : ControllerBase
{
    private readonly ILocalHireInfoService _localHireInfoService;
    private readonly IWelcomeMessageService _welcomeMessageService;
    private readonly ILogger<ContentController> _logger;

    public ContentController(ILocalHireInfoService localHireInfoService, IWelcomeMessageService welcomeMessageService, ILogger<ContentController> logger)
    {
        _localHireInfoService = localHireInfoService;
        _welcomeMessageService = welcomeMessageService;
        _logger = logger;
    }

    [HttpPost("local-hire-info-upload")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<LocalHireInfoResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> UploadLocalHireInfo(IFormFile file)
    {
        _logger.LogInformation("Local hire info upload request received");

        return (file == null || file.Length == 0) ? BadRequest(ApiResponse<object>.Failure("File is required")) :
            file.FileName.EndsWith(".xlsx") || file.FileName.EndsWith(".xls") || file.FileName.EndsWith(".csv") || file.FileName.EndsWith(".excel") 
            || file.FileName.EndsWith(".Excel") || file.FileName.EndsWith(".CSV") || file.FileName.EndsWith(".EXCEL") || file.FileName.EndsWith(".XLSX") || file.FileName.EndsWith(".XLS") ? Ok(ApiResponse<LocalHireInfoResponse>.Success(await _localHireInfoService.ExtractAndSaveLocalHireInfoAsync(file))) :
           BadRequest(ApiResponse<object>.Failure("Invalid file format"));

    }

    [HttpGet("get-local-hire-info")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<LocalHireInfoResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetLocalHireInfo()
    {
        _logger.LogInformation("Get local hire info request received");

            var result = await _localHireInfoService.GetLocalHireInfoAsync();
            return result == null ? NotFound(ApiResponse<object>.Failure("Local hire info not found")) : Ok(ApiResponse<LocalHireInfoResponse>.Success(result));

    }

    [HttpPost("upload-welcome-messages")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<WelcomeMessageResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> UploadWelcomeMessages(IFormFileCollection files)
    {
        _logger.LogInformation("Upload welcome messages request received");

        if (files == null || files.Count == 0)
            return BadRequest(ApiResponse<object>.Failure("Files are required"));

        if (files.Any(f => !f.FileName.EndsWith(".docx") && !f.FileName.EndsWith(".doc")))
            return BadRequest(ApiResponse<object>.Failure("Only .docx and .doc files are allowed"));

        return Ok(ApiResponse<WelcomeMessageResponse>.Success(await _welcomeMessageService.ExtractAndSaveWelcomeMessagesAsync(files)));
    }

    [HttpGet("get-welcome-messages")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<WelcomeMessageResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetWelcomeMessages()
    {
        _logger.LogInformation("Get welcome messages request received");

        var result = await _welcomeMessageService.GetWelcomeMessagesAsync();
        return result == null ? NotFound(ApiResponse<object>.Failure("Welcome messages not found")) : Ok(ApiResponse<WelcomeMessageResponse>.Success(result));
    }
}