using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Lafarge_Onboarding.application.Abstraction;
using Lafarge_Onboarding.domain.OnboardingRequests;
using Lafarge_Onboarding.domain.OnboardingResponses;

namespace Lafarge_Onboarding.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
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
    public async Task<IActionResult> Register([FromBody] AuthRegisterRequest request)
    {
        _logger.LogInformation("Registration attempt for email: {Email}", request.Email);

        var result = await _authService.RegisterUserAsync(request);
        _logger.LogInformation("User registered successfully: {Email}", request.Email);
        return Ok(ApiResponse<AuthRegisterResponse>.Success(result, "User registered successfully"));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] AuthLoginRequest request)
    {
        _logger.LogInformation("Login attempt for email: {Email}", request.Email);

        var result = await _authService.LoginUserAsync(request);
        if (result == null)
        {
            _logger.LogWarning("Invalid login attempt for email: {Email}", request.Email);
            return Unauthorized(ApiResponse<AuthLoginResponse>.Failure("Invalid credentials"));
        }

        _logger.LogInformation("Login successful for user: {UserId}", result.User.Id);
        return Ok(ApiResponse<AuthLoginResponse>.Success(result, "Login successful"));
    }
}