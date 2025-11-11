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
      return !ModelState.IsValid ?

            BadRequest(ApiResponse<object>.Failure(string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()))) :

            Ok(ApiResponse<AuthRegisterResponse>.Success(await _authService.RegisterUserAsync(request)));
    }



    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthLoginResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<AuthLoginResponse>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> Login([FromBody] AuthLoginRequest request)
    {
        return !ModelState.IsValid 
            ? BadRequest(ApiResponse<object>.Failure(string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))))
            : (await _authService.LoginUserAsync(request)) == null
                ? Unauthorized(ApiResponse<AuthLoginResponse>.Failure("Invalid credentials", "401"))
                : Ok(ApiResponse<AuthLoginResponse>.Success((await _authService.LoginUserAsync(request))!));
    }



    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.Failure(string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList())));
        }

        try
        {
            var result = await _authService.ForgotPasswordAsync(request);
            return Ok(ApiResponse<string>.Success(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Failure(ex.Message));
        }
    }



    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.Failure(string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList())));
        }

        try
        {
            var result = await _authService.ResetPasswordAsync(request);
            return Ok(ApiResponse<string>.Success(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Failure(ex.Message));
        }
    }
}
    