namespace Lafarge_Onboarding.application.Abstraction;

public interface IGalleryRepository
{
    Task AddAsync(Gallery gallery);
    Task<List<Gallery>> GetByImageTypeAsync(string imageType);
    Task DeleteByImageTypeAsync(string imageType);
    Task<Gallery?> GetByIdAsync(int id);
    Task DeleteByIdAsync(int id);
}