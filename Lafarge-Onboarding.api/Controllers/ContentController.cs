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

    [HttpPost("upload-onboarding-plan")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<OnboardingPlanResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> UploadOnboardingPlan(IFormFile file)
    {
        _logger.LogInformation("Upload onboarding plan request received");

        if (file == null || file.Length == 0)
            return BadRequest(ApiResponse<object>.Failure("File is required"));

        if (!file.FileName.EndsWith(".docx") && !file.FileName.EndsWith(".doc"))
            return BadRequest(ApiResponse<object>.Failure("Only .docx and .doc files are allowed"));

        return Ok(ApiResponse<OnboardingPlanResponse>.Success(await _onboardingPlanService.ExtractAndSaveOnboardingPlanAsync(file)));
    }

    [HttpGet("get-onboarding-plan")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<OnboardingPlanResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetOnboardingPlan()
    {
        _logger.LogInformation("Get onboarding plan request received");

        var result = await _onboardingPlanService.GetOnboardingPlanAsync();
        return result == null ? NotFound(ApiResponse<object>.Failure("Onboarding plan not found")) : Ok(ApiResponse<OnboardingPlanResponse>.Success(result));
    }

    [HttpPost("upload-etiquette")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<EtiquetteResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> UploadEtiquette(IFormFile file)
    {
        _logger.LogInformation("Upload etiquette request received");

        if (file == null || file.Length == 0)
            return BadRequest(ApiResponse<object>.Failure("File is required"));

        if (!file.FileName.EndsWith(".docx") && !file.FileName.EndsWith(".doc"))
            return BadRequest(ApiResponse<object>.Failure("Only .docx and .doc files are allowed"));

        return Ok(ApiResponse<EtiquetteResponse>.Success(await _etiquetteService.ExtractAndSaveEtiquetteAsync(file)));
    }

    [HttpGet("get-etiquette")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<EtiquetteResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetEtiquette()
    {
        _logger.LogInformation("Get etiquette request received");

        var result = await _etiquetteService.GetEtiquetteAsync();
        return result == null ? NotFound(ApiResponse<object>.Failure("Etiquette not found")) : Ok(ApiResponse<EtiquetteResponse>.Success(result));
    }
}