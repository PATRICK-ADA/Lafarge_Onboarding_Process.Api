namespace Lafarge_Onboarding.application.Abstraction;

public interface IAuthService
{
    Task<AuthRegisterResponse> RegisterUserAsync(AuthRegisterRequest request);
    Task<AuthLoginResponse?> LoginUserAsync(AuthLoginRequest request);
    Task<string> GenerateJwtToken(Users user);
    Task<Users?> ValidateUserCredentials(string email, string password);
    Task<bool> AssignRoleToUserAsync(string userId, string roleName);
    Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordRequest request);
}