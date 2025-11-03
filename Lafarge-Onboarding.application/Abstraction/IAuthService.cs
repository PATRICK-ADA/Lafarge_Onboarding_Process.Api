using Lafarge_Onboarding.domain.Entities;
using Lafarge_Onboarding.domain.OnboardingRequests;
using Lafarge_Onboarding.domain.OnboardingResponses;

namespace Lafarge_Onboarding.application.Abstraction;

public interface IAuthService
{
    Task<AuthRegisterResponse> RegisterUserAsync(AuthRegisterRequest request);
    Task<AuthLoginResponse?> LoginUserAsync(AuthLoginRequest request);
    Task<string> GenerateJwtToken(Users user);
    Task<Users?> ValidateUserCredentials(string email, string password);
    Task<bool> AssignRoleToUserAsync(string userId, string roleName);
}