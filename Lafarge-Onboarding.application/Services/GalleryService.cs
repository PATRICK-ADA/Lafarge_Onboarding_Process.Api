
namespace Lafarge_Onboarding.application.Services;

public sealed class GalleryService : IGalleryService
{
    private readonly IGalleryRepository _repository;
    private readonly ILogger<GalleryService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuditService _auditService;

    public GalleryService(IGalleryRepository repository, ILogger<GalleryService> logger, IHttpContextAccessor httpContextAccessor, IAuditService auditService)
    {
        _repository = repository;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _auditService = auditService;
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

        await _auditService.LogAuditEventAsync(
            action: "UPLOAD",
            resourceType: "Gallery",
            resourceId: gallery.Id.ToString(),
            description: $"Uploaded {imageType} image: {gallery.ImageName}",
            newValues: JsonSerializer.Serialize(new { gallery.Id, gallery.ImageName, gallery.ImageType }));

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

    public async Task<List<GalleryResponse>> GetCeoImagesAsync()
    {
        _logger.LogInformation("Retrieving CEO images");
        var images = await _repository.GetByImageTypeAsync("CEO");

        await _auditService.LogAuditEventAsync(
            action: "RETRIEVE",
            resourceType: "Gallery",
            resourceId: "CEO",
            description: $"Retrieved {images.Count} CEO images");

        return images;
    }

    public async Task<List<GalleryResponse>> GetHrImagesAsync()
    {
        _logger.LogInformation("Retrieving HR images");
        var images = await _repository.GetByImageTypeAsync("HR");

        await _auditService.LogAuditEventAsync(
            action: "RETRIEVE",
            resourceType: "Gallery",
            resourceId: "HR",
            description: $"Retrieved {images.Count} HR images");

        return images;
    }

    public async Task<List<GalleryResponse>> GetAnyImagesAsync()
    {
        _logger.LogInformation("Retrieving general images");
        var images = await _repository.GetByImageTypeAsync("GENERAL");

        await _auditService.LogAuditEventAsync(
            action: "RETRIEVE",
            resourceType: "Gallery",
            resourceId: "GENERAL",
            description: $"Retrieved {images.Count} general images");

        return images;
    }

    public async Task<string> DeleteCeoImagesAsync()
    {
        _logger.LogInformation("Deleting all CEO images");
        var oldImages = await _repository.GetByImageTypeAsync("CEO");
        await _repository.DeleteByImageTypeAsync("CEO");

        await _auditService.LogAuditEventAsync(
            action: "DELETE",
            resourceType: "Gallery",
            resourceId: "CEO",
            description: $"Deleted {oldImages.Count} CEO images",
            oldValues: JsonSerializer.Serialize(oldImages.Select(i => new { i.Id, i.ImageName })));

        return "CEO images deleted successfully";
    }

    public async Task<string> DeleteHrImagesAsync()
    {
        _logger.LogInformation("Deleting all HR images");
        var oldImages = await _repository.GetByImageTypeAsync("HR");
        await _repository.DeleteByImageTypeAsync("HR");

        await _auditService.LogAuditEventAsync(
            action: "DELETE",
            resourceType: "Gallery",
            resourceId: "HR",
            description: $"Deleted {oldImages.Count} HR images",
            oldValues: JsonSerializer.Serialize(oldImages.Select(i => new { i.Id, i.ImageName })));

        return "HR images deleted successfully";
    }

    public async Task<string> DeleteAnyImagesAsync()
    {
        _logger.LogInformation("Deleting all general images");
        var oldImages = await _repository.GetByImageTypeAsync("GENERAL");
        await _repository.DeleteByImageTypeAsync("GENERAL");

        await _auditService.LogAuditEventAsync(
            action: "DELETE",
            resourceType: "Gallery",
            resourceId: "GENERAL",
            description: $"Deleted {oldImages.Count} general images",
            oldValues: JsonSerializer.Serialize(oldImages.Select(i => new { i.Id, i.ImageName })));

        return "General images deleted successfully";
    }

    public async Task<string> DeleteImageByIdAsync(int id)
    {
        _logger.LogInformation("Deleting image with ID: {Id}", id);
        var image = await _repository.GetByIdAsync(id);
        if (image == null)
        {
            throw new KeyNotFoundException($"Image with ID {id} not found");
        }

        await _repository.DeleteByIdAsync(id);

        await _auditService.LogAuditEventAsync(
            action: "DELETE",
            resourceType: "Gallery",
            resourceId: id.ToString(),
            description: $"Deleted image: {image.ImageName}",
            oldValues: JsonSerializer.Serialize(new { image.Id, image.ImageName, image.ImageType }));

        return "Image deleted successfully";
    }
}