namespace Lafarge_Onboarding.application.Abstraction;

public interface IGalleryService
{
    Task<string> UploadImageAsync(IFormFile image, string imageType);
    Task<List<Gallery>> GetCeoImagesAsync();
    Task<List<Gallery>> GetHrImagesAsync();
    Task<List<Gallery>> GetAnyImagesAsync();
    Task<string> DeleteCeoImagesAsync();
    Task<string> DeleteHrImagesAsync();
    Task<string> DeleteAnyImagesAsync();
    Task<string> DeleteImageByIdAsync(int id);
}