namespace Lafarge_Onboarding.application.Abstraction;

public interface IGalleryService
{
    Task<string> UploadImageAsync(IFormFile image, string imageType);
    Task<List<GalleryResponse>> GetCeoImagesAsync();
    Task<List<GalleryResponse>> GetHrImagesAsync();
    Task<List<GalleryResponse>> GetAnyImagesAsync();
    Task<string> DeleteCeoImagesAsync();
    Task<string> DeleteHrImagesAsync();
    Task<string> DeleteAnyImagesAsync();
    Task<string> DeleteImageByIdAsync(int id);
}