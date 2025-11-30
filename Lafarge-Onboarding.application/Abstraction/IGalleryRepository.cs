namespace Lafarge_Onboarding.application.Abstraction;

public interface IGalleryRepository
{
    Task AddAsync(Gallery gallery);
    Task<List<GalleryResponse>> GetByImageTypeAsync(string imageType);
    Task DeleteByImageTypeAsync(string imageType);
    Task<GalleryResponse?> GetByIdAsync(int id);
    Task DeleteByIdAsync(int id);
}