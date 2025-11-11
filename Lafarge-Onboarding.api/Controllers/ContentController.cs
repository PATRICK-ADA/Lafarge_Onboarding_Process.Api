namespace Lafarge_Onboarding.api.Controllers;

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
}