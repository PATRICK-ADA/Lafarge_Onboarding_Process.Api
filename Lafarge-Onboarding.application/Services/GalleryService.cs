using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System.Security.Claims;

namespace Lafarge_Onboarding.application.Services;

public sealed class GalleryService : IGalleryService
{
    private readonly IGalleryRepository _repository;
    private readonly ILogger<GalleryService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GalleryService(IGalleryRepository repository, ILogger<GalleryService> logger, IHttpContextAccessor httpContextAccessor)
    {
        _repository = repository;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> UploadImageAsync(IFormFile image, string imageType)
    {
        _logger.LogInformation("Uploading {ImageType} image: {FileName}", imageType, image.FileName);

        var compressedBytes = await CompressImageAsync(image);
        var base64String = Convert.ToBase64String(compressedBytes);

        var currentUser = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
        
        var gallery = new Gallery
        {
            ImageName = image.FileName,
            ImageBase64 = base64String,
            ImageType = imageType,
            UploadedAt = DateTime.UtcNow,
            UploadedBy = currentUser
        };

        await _repository.AddAsync(gallery);
        
        _logger.LogInformation("Successfully uploaded {ImageType} image with ID: {Id}", imageType, gallery.Id);
        return $"{imageType} image uploaded successfully";
    }

    private static async Task<byte[]> CompressImageAsync(IFormFile image)
    {
        using var inputStream = new MemoryStream();
        await image.CopyToAsync(inputStream);
        inputStream.Position = 0;

        using var img = await Image.LoadAsync(inputStream);
        using var outputStream = new MemoryStream();

        var format = img.Metadata.DecodedImageFormat;
        if (format?.Name == "JPEG")
        {
            await img.SaveAsJpegAsync(outputStream, new JpegEncoder { Quality = 60 });
        }
        else if (format?.Name == "PNG")
        {
            await img.SaveAsPngAsync(outputStream, new PngEncoder { CompressionLevel = PngCompressionLevel.Level9 });
        }
        else
        {
            await img.SaveAsJpegAsync(outputStream, new JpegEncoder { Quality = 60 });
        }

        return outputStream.ToArray();
    }

    public async Task<List<Gallery>> GetCeoImagesAsync()
    {
        _logger.LogInformation("Retrieving CEO images");
        return await _repository.GetByImageTypeAsync("CEO");
    }

    public async Task<List<Gallery>> GetHrImagesAsync()
    {
        _logger.LogInformation("Retrieving HR images");
        return await _repository.GetByImageTypeAsync("HR");
    }

    public async Task<List<Gallery>> GetAnyImagesAsync()
    {
        _logger.LogInformation("Retrieving general images");
        return await _repository.GetByImageTypeAsync("GENERAL");
    }

    public async Task<string> DeleteCeoImagesAsync()
    {
        _logger.LogInformation("Deleting all CEO images");
        await _repository.DeleteByImageTypeAsync("CEO");
        return "CEO images deleted successfully";
    }

    public async Task<string> DeleteHrImagesAsync()
    {
        _logger.LogInformation("Deleting all HR images");
        await _repository.DeleteByImageTypeAsync("HR");
        return "HR images deleted successfully";
    }

    public async Task<string> DeleteAnyImagesAsync()
    {
        _logger.LogInformation("Deleting all general images");
        await _repository.DeleteByImageTypeAsync("GENERAL");
        return "General images deleted successfully";
    }

    public async Task<string> DeleteImageByIdAsync(int id)
    {
        _logger.LogInformation("Deleting image with ID: {Id}", id);
        var image = await _repository.GetByIdAsync(id);
        return image == null 
            ? throw new KeyNotFoundException($"Image with ID {id} not found")
            : await Task.Run(async () => { await _repository.DeleteByIdAsync(id); return "Image deleted successfully"; });
    }
}