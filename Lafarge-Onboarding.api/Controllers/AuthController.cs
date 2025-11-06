namespace Lafarge_Onboarding.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthRegisterResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> Register([FromBody] AuthRegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<object>.Failure(string.Join("; ", errors)));
        }

        var result = await _authService.RegisterUserAsync(request);
    
        return Ok(ApiResponse<AuthRegisterResponse>.Success(result));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthLoginResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<AuthLoginResponse>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> Login([FromBody] AuthLoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<object>.Failure(string.Join("; ", errors)));
        }

        var result = await _authService.LoginUserAsync(request);
        if (result == null)
        {
            return Unauthorized(ApiResponse<AuthLoginResponse>.Failure("Invalid credentials"));
        }

        return Ok(ApiResponse<AuthLoginResponse>.Success(result));
    }
}