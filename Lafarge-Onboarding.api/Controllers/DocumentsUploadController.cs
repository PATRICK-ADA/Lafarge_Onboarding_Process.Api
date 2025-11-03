using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Lafarge_Onboarding.application.Abstraction;
using Lafarge_Onboarding.domain.OnboardingResponses;
using Lafarge_Onboarding.domain.OnboardingRequests;
using Lafarge_Onboarding.domain.Entities;

namespace Lafarge_Onboarding.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DocumentsUploadController : ControllerBase
{
    private readonly IDocumentsUploadService _uploadService;
    private readonly ILogger<DocumentsUploadController> _logger;

    public DocumentsUploadController(IDocumentsUploadService uploadService, ILogger<DocumentsUploadController> logger)
    {
        _uploadService = uploadService;
        _logger = logger;
    }

    [HttpPost("upload/welcome-message-CEO")]
    [Authorize(Roles = "HR_ADMIN")]
    public async Task<IActionResult> UploadWelcomeMessageCEO(IFormFile file)
    {
        return await UploadDocument(file, "welcome-message-CEO");
    }

    [HttpPost("upload/welcome-message-HR")]
    [Authorize(Roles = "HR_ADMIN")]
    public async Task<IActionResult> UploadWelcomeMessageHR(IFormFile file)
    {
        return await UploadDocument(file, "welcome-message-HR");
    }

    [HttpPost("upload/about-lafarge")]
    [Authorize(Roles = "HR_ADMIN")]
    public async Task<IActionResult> UploadAboutLafarge(IFormFile file)
    {
        return await UploadDocument(file, "about-lafarge");
    }

    [HttpPost("upload/welcome-to-nigeria")]
    [Authorize(Roles = "HR_ADMIN")]
    public async Task<IActionResult> UploadWelcomeToNigeria(IFormFile file)
    {
        return await UploadDocument(file, "welcome-to-nigeria");
    }

    [HttpPost("upload/key-contacts")]
    [Authorize(Roles = "HR_ADMIN")]
    public async Task<IActionResult> UploadKeyContacts(IFormFile file)
    {
        return await UploadDocument(file, "key-contacts");
    }

    [HttpPost("upload/etiquette-&&-faux-pas")]
    [Authorize(Roles = "HR_ADMIN")]
    public async Task<IActionResult> UploadEtiquetteFauxPas(IFormFile file)
    {
        return await UploadDocument(file, "etiquette-&&-faux-pas");
    }

    [HttpPost("upload/gallery")]
    [Authorize(Roles = "HR_ADMIN")]
    public async Task<IActionResult> UploadGallery(IFormFile file)
    {
        return await UploadDocument(file, "gallery");
    }

    [HttpPost("upload")]
    [Authorize(Roles = "HR_ADMIN")]
    public async Task<IActionResult> UploadGenericDocument(IFormFile file, [FromForm] string documentType)
    {
        return await UploadDocument(file, documentType);
    }

    [HttpPost("upload/bulk")]
    [Authorize(Roles = "HR_ADMIN")]
    public async Task<IActionResult> UploadBulkDocuments(List<IFormFile> files, [FromForm] string documentType)
    {
        _logger.LogInformation("Bulk document upload request received for type: {DocumentType}, file count: {FileCount}", documentType, files.Count);

        // Get the authenticated user's ID
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Unauthorized bulk upload attempt - no user ID found");
            return Unauthorized(ApiResponse<IEnumerable<DocumentUploadResponse>>.Failure("User not authenticated"));
        }
        _logger.LogInformation("Processing bulk upload for user: {UserId}", userId);

        var responses = await _uploadService.ProcessDocumentsBulkAsync(files, documentType, userId);

        return Ok(ApiResponse<IEnumerable<DocumentUploadResponse>>.Success(responses, $"Bulk upload completed successfully for {documentType}."));
    }

    [HttpGet("all")]
    [Authorize]
    public async Task<IActionResult> GetAllDocuments([FromQuery] PaginationRequest request)
    {
        _logger.LogInformation("Get all documents request received. Page: {PageNumber}, Size: {PageSize}", request.PageNumber, request.PageSize);

        var response = await _uploadService.GetAllDocumentsPaginatedAsync(request);

        return Ok(ApiResponse<PaginatedResponse<DocumentUploadResponse>>.Success(response, "Documents retrieved successfully."));
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetDocumentById(int id)
    {
        _logger.LogInformation("Get document by ID request received. ID: {DocumentId}", id);

        var document = await _uploadService.GetDocumentByIdAsync(id);
        if (document == null)
        {
            _logger.LogWarning("Document not found. ID: {DocumentId}", id);
            return NotFound(ApiResponse<DocumentUploadResponse>.Failure("Document not found."));
        }

        var response = new DocumentUploadResponse
        {
            Message = "Document retrieved successfully.",
            DocumentId = document.Id,
            FileName = document.FileName,
            FilePath = document.FilePath,
            ContentLength = document.Content.Length,
            Content = document.Content
        };

        return Ok(ApiResponse<DocumentUploadResponse>.Success(response, "Document retrieved successfully."));
    }

    private async Task<IActionResult> UploadDocument(IFormFile file, string documentType)
    {
        _logger.LogInformation("Document upload request received for type: {DocumentType}", documentType);

        // Get the authenticated user's ID
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Unauthorized upload attempt - no user ID found");
            return Unauthorized(ApiResponse<DocumentUploadResponse>.Failure("User not authenticated"));
        }
        _logger.LogInformation("Processing upload for user: {UserId}", userId);

        var response = await _uploadService.ProcessDocumentUploadAsync(file, documentType, userId);

        return Ok(ApiResponse<DocumentUploadResponse>.Success(response, $"File uploaded successfully for {documentType}."));
    }
}