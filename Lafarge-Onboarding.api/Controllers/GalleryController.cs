namespace Lafarge_Onboarding.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class GalleryController : ControllerBase
{
    private readonly IGalleryService _galleryService;
    private readonly ILogger<GalleryController> _logger;

    public GalleryController(IGalleryService galleryService, ILogger<GalleryController> logger)
    {
        _galleryService = galleryService;
        _logger = logger;
    }

    [HttpPost("upload-ceo-image")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> UploadCeoImage(IFormFile image)
    {
        return (image == null || image.Length == 0) 
            ? BadRequest(ApiResponse<object>.Failure("Image file is required"))
            : !IsValidImageFile(image)
                ? BadRequest(ApiResponse<object>.Failure("Invalid image format. Only JPG, JPEG, PNG, GIF are allowed"))
                : Ok(ApiResponse<string>.Success(await _galleryService.UploadImageAsync(image, "CEO")));
    }

    [HttpPost("upload-hr-image")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> UploadHrImage(IFormFile image)
    {
        return (image == null || image.Length == 0) 
            ? BadRequest(ApiResponse<object>.Failure("Image file is required"))
            : !IsValidImageFile(image)
                ? BadRequest(ApiResponse<object>.Failure("Invalid image format. Only JPG, JPEG, PNG, GIF are allowed"))
                : Ok(ApiResponse<string>.Success(await _galleryService.UploadImageAsync(image, "HR")));
    }

    [HttpPost("upload-any-image")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> UploadAnyImage(IFormFile image)
    {
        return (image == null || image.Length == 0) 
            ? BadRequest(ApiResponse<object>.Failure("Image file is required"))
            : !IsValidImageFile(image)
                ? BadRequest(ApiResponse<object>.Failure("Invalid image format. Only JPG, JPEG, PNG, GIF are allowed"))
                : Ok(ApiResponse<string>.Success(await _galleryService.UploadImageAsync(image, "GENERAL")));
    }

    [HttpGet("get-ceo-images")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<List<GalleryResponse>>), 200)]
    public async Task<IActionResult> GetCeoImages()
    {
        var images = await _galleryService.GetCeoImagesAsync();
        return images.Count == 0 ? NotFound(ApiResponse<object>.Failure("Ceo Image not found", "404")) : Ok(ApiResponse<List<GalleryResponse>>.Success(images));
    }

    [HttpGet("get-hr-images")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<List<GalleryResponse>>), 200)]
    public async Task<IActionResult> GetHrImages()
    {
        var images = await _galleryService.GetHrImagesAsync();
        return images.Count == 0 ? NotFound(ApiResponse<object>.Failure("Hr Image not found", "404")) : Ok(ApiResponse<List<GalleryResponse>>.Success(images));
    }

    [HttpGet("get-any-images")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<List<GalleryResponse>>), 200)]
    public async Task<IActionResult> GetAnyImages()
    {
        var images = await _galleryService.GetAnyImagesAsync();
        return images.Count == 0 ? NotFound(ApiResponse<object>.Failure("Image not found", "404")) : Ok(ApiResponse<List<GalleryResponse>>.Success(images));
    }

    [HttpDelete("delete-ceo-images")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    public async Task<IActionResult> DeleteCeoImages()
    {
        var result = await _galleryService.DeleteCeoImagesAsync();
        return Ok(ApiResponse<string>.Success(result));
    }

    [HttpDelete("delete-hr-images")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    public async Task<IActionResult> DeleteHrImages()
    {
        var result = await _galleryService.DeleteHrImagesAsync();
        return Ok(ApiResponse<string>.Success(result));
    }

    [HttpDelete("delete-any-images")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    public async Task<IActionResult> DeleteAnyImages()
    {
        var result = await _galleryService.DeleteAnyImagesAsync();
        return Ok(ApiResponse<string>.Success(result));
    }

    [HttpDelete("delete-image/{id}")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> DeleteImageById(int id)
    {
        var result = await _galleryService.DeleteImageByIdAsync(id);
        return Ok(ApiResponse<string>.Success(result));
    }

    private static bool IsValidImageFile(IFormFile file)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return allowedExtensions.Contains(fileExtension);
    }
}