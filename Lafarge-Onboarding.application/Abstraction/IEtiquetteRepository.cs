namespace Lafarge_Onboarding.application.Abstraction;

public interface IEtiquetteRepository
{
    Task AddAsync(Etiquette etiquette);
    Task<EtiquetteResponse?> GetLatestAsync();
    Task UpdateAsync(Etiquette etiquette);
    Task DeleteLatestAsync();
    Task DeleteAllAsync();
}