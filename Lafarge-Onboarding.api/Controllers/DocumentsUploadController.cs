namespace Lafarge_Onboarding.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class DocumentsUploadController : ControllerBase
{
    private readonly IDocumentsUploadService _uploadService;
    private readonly ILogger<DocumentsUploadController> _logger;

    public DocumentsUploadController(IDocumentsUploadService uploadService, ILogger<DocumentsUploadController> logger)
    {
        _uploadService = uploadService;
        _logger = logger;
    }

    [HttpPost("upload")]
    [Authorize(Roles = "HR_ADMIN")]
    public async Task<IActionResult> UploadDocument([FromForm] DocumentUploadRequest request)
    {
        _logger.LogInformation("Document upload request received");

        if (request.ContentBodyUpload == null)
        {
            _logger.LogWarning("No file provided in ContentBodyUpload");
            return BadRequest(ApiResponse<DocumentUploadResponse>.Failure("ContentBodyUpload file is required"));
        }

        // Get the authenticated user's ID
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Unauthorized upload attempt - no user ID found");
            return Unauthorized(ApiResponse<DocumentUploadResponse>.Failure("User not authenticated"));
        }
        _logger.LogInformation("Processing upload for user: {UserId}", userId);

        var response = await _uploadService.ProcessDocumentUploadAsync(request.ContentBodyUpload, userId, request.ContentHeading, request.ContentSubHeading, request.ImageUpload);

        return Ok(ApiResponse<DocumentUploadResponse>.Success(response));
    }

    [HttpPost("upload/bulk")]
    [Authorize(Roles = "HR_ADMIN")]
    public async Task<IActionResult> UploadBulkDocuments([FromForm] List<DocumentUploadRequest> requests)
    {
        _logger.LogInformation("Bulk document upload request received, request count: {RequestCount}", requests.Count);

        // Get the authenticated user's ID
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Unauthorized bulk upload attempt - no user ID found");
            return Unauthorized(ApiResponse<IEnumerable<DocumentUploadResponse>>.Failure("User not authenticated"));
        }
        _logger.LogInformation("Processing bulk upload for user: {UserId}", userId);

        var files = requests.Where(r => r.ContentBodyUpload != null).Select(r => r.ContentBodyUpload!).ToList();
        var contentHeading = requests.FirstOrDefault()?.ContentHeading ?? string.Empty;
        var contentSubHeading = requests.FirstOrDefault()?.ContentSubHeading ?? string.Empty;
        var responses = await _uploadService.ProcessDocumentsBulkAsync(files, userId, contentHeading, contentSubHeading);

        return Ok(ApiResponse<IEnumerable<DocumentUploadResponse>>.Success(responses));
    }

    [HttpGet("all")]
    [Authorize]
    public async Task<IActionResult> GetAllDocuments([FromQuery] PaginationRequest request)
    {
        _logger.LogInformation("Get all documents request received. Page: {PageNumber}, Size: {PageSize}", request.PageNumber, request.PageSize);

        var response = await _uploadService.GetAllDocumentsPaginatedAsync(request);

        return Ok(ApiResponse<PaginatedResponse<DocumentUploadResponse>>.Success(response));
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
            BodyContentFileType = document.FileName,
            BodyFilePath = document.FilePath,
            BodyContent = document.Content,
            ImageFilePath = document.ImageFilePath,
            ImageFileType = document.ImageFileType,
            ContentHeading = document.ContentHeading,
            ContentSubHeading = document.ContentSubHeading
        };

        return Ok(ApiResponse<DocumentUploadResponse>.Success(response));
    }

}