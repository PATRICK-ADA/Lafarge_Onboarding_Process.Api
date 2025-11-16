namespace Lafarge_Onboarding.application.Abstraction;
public interface IEtiquetteService
{
    Task<EtiquetteResponse> ExtractAndSaveEtiquetteAsync(IFormFile file);
    Task<EtiquetteResponse?> GetEtiquetteAsync();
    Task DeleteLatestAsync();
}