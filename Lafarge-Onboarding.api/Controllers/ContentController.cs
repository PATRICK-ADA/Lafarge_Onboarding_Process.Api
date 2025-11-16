namespace Lafarge_Onboarding.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ContentController : ControllerBase
{
    private readonly ILocalHireInfoService _localHireInfoService;
    private readonly IWelcomeMessageService _welcomeMessageService;
    private readonly IOnboardingPlanService _onboardingPlanService;
    private readonly IEtiquetteService _etiquetteService;
    private readonly ILogger<ContentController> _logger;

    public ContentController(ILocalHireInfoService localHireInfoService, IWelcomeMessageService welcomeMessageService, IOnboardingPlanService onboardingPlanService, IEtiquetteService etiquetteService, ILogger<ContentController> logger)
    {
        _localHireInfoService = localHireInfoService;
        _welcomeMessageService = welcomeMessageService;
        _onboardingPlanService = onboardingPlanService;
        _etiquetteService = etiquetteService;
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
           Ok(ApiResponse<LocalHireInfoResponse>.Success(await _localHireInfoService.ExtractAndSaveLocalHireInfoAsync(file)));

    }

    [HttpGet("get-local-hire-info")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<LocalHireInfoResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetLocalHireInfo()
    {
        _logger.LogInformation("Get local hire info request received");

            var result = await _localHireInfoService.GetLocalHireInfoAsync();
            return result == null ? NotFound(ApiResponse<object>.Failure("Local hire info not found", "404")) : Ok(ApiResponse<LocalHireInfoResponse>.Success(result));

    }

    [HttpPost("upload-welcome-messages")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<WelcomeMessageResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> UploadWelcomeMessages(IFormFileCollection files)
    {
        _logger.LogInformation("Upload welcome messages request received");

        return (files == null || files.Count == 0)
            ? BadRequest(ApiResponse<object>.Failure("Files are required"))
            : files.Any(f => !f.FileName.EndsWith(".docx") && !f.FileName.EndsWith(".doc"))
                ? BadRequest(ApiResponse<object>.Failure("Only .docx and .doc files are allowed"))
                : Ok(ApiResponse<WelcomeMessageResponse>.Success(await _welcomeMessageService.ExtractAndSaveWelcomeMessagesAsync(files)));
    }

    [HttpGet("get-welcome-messages")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<WelcomeMessageResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetWelcomeMessages()
    {
        _logger.LogInformation("Get welcome messages request received");

        var result = await _welcomeMessageService.GetWelcomeMessagesAsync();
        return result == null ? NotFound(ApiResponse<object>.Failure("Welcome messages not found", "404")) : Ok(ApiResponse<WelcomeMessageResponse>.Success(result));
    }

    [HttpPost("upload-onboarding-plan")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<OnboardingPlanResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> UploadOnboardingPlan(IFormFile file)
    {
        _logger.LogInformation("Upload onboarding plan request received");

        return (file == null || file.Length == 0)
            ? BadRequest(ApiResponse<object>.Failure("File is required"))
            : (!file.FileName.EndsWith(".docx") && !file.FileName.EndsWith(".doc"))
                ? BadRequest(ApiResponse<object>.Failure("Only .docx and .doc files are allowed"))
                : Ok(ApiResponse<OnboardingPlanResponse>.Success(await _onboardingPlanService.ExtractAndSaveOnboardingPlanAsync(file)));
    }

    [HttpGet("get-onboarding-plan")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<OnboardingPlanResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetOnboardingPlan()
    {
        _logger.LogInformation("Get onboarding plan request received");

        var result = await _onboardingPlanService.GetOnboardingPlanAsync();
        return result == null ? NotFound(ApiResponse<object>.Failure("Onboarding plan not found", "404")) : Ok(ApiResponse<OnboardingPlanResponse>.Success(result));
    }

    [HttpPost("upload-etiquette")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<EtiquetteResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> UploadEtiquette(IFormFile file)
    {
        _logger.LogInformation("Upload etiquette request received");

        return (file == null || file.Length == 0)
            ? BadRequest(ApiResponse<object>.Failure("File is required"))
            : (!file.FileName.EndsWith(".docx") && !file.FileName.EndsWith(".doc"))
                ? BadRequest(ApiResponse<object>.Failure("Only .docx and .doc files are allowed"))
                : Ok(ApiResponse<EtiquetteResponse>.Success(await _etiquetteService.ExtractAndSaveEtiquetteAsync(file)));
    }

    [HttpGet("get-etiquette")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<EtiquetteResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetEtiquette()
    {
        _logger.LogInformation("Get etiquette request received");

        var result = await _etiquetteService.GetEtiquetteAsync();
        return result == null ? NotFound(ApiResponse<object>.Failure("Etiquette not found", "404")) : Ok(ApiResponse<EtiquetteResponse>.Success(result));
    }

    [HttpDelete("delete-local-hire-info")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> DeleteLocalHireInfo()
    {
        _logger.LogInformation("Delete latest local hire info request received");

        await _localHireInfoService.DeleteLatestAsync();
        return Ok(ApiResponse<object>.Success(null, "Local hire info deleted successfully"));
    }

    [HttpDelete("delete-welcome-messages")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> DeleteWelcomeMessages()
    {
        _logger.LogInformation("Delete latest welcome messages request received");

        await _welcomeMessageService.DeleteLatestAsync();
        return Ok(ApiResponse<object>.Success(null, "Welcome messages deleted successfully"));
    }

    [HttpDelete("delete-onboarding-plan")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> DeleteOnboardingPlan()
    {
        _logger.LogInformation("Delete latest onboarding plan request received");

        await _onboardingPlanService.DeleteLatestAsync();
        return Ok(ApiResponse<object>.Success(null, "Onboarding plan deleted successfully"));
    }

    [HttpDelete("delete-etiquette")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> DeleteEtiquette()
    {
        _logger.LogInformation("Delete latest etiquette request received");

        await _etiquetteService.DeleteLatestAsync();
        return Ok(ApiResponse<object>.Success("Etiquette deleted successfully"));
    }
}