namespace Lafarge_Onboarding.application.Abstraction;

using Lafarge_Onboarding.domain.Dtos.OnboardingResponses;

public interface IEtiquetteService
{
    Task<EtiquetteResponse> ExtractAndSaveEtiquetteAsync(IFormFile file);
    Task<EtiquetteResponse?> GetEtiquetteAsync();
    Task DeleteLatestAsync();
}