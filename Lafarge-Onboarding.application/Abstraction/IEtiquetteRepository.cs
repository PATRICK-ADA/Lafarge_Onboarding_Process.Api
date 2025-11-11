namespace Lafarge_Onboarding.application.Abstraction;

public interface IEtiquetteRepository
{
    Task AddAsync(Etiquette etiquette);
    Task<Etiquette?> GetLatestAsync();
    Task UpdateAsync(Etiquette etiquette);
}