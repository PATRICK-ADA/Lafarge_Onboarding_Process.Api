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
    
      return   request.ContentBodyUpload == null ?

            BadRequest(ApiResponse<DocumentUploadResponse>.Failure("ContentBodyUpload file is required")) :

         User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value == null ?
    
         Unauthorized(ApiResponse<DocumentUploadResponse>.Failure("User not authenticated")) :

        Ok(await _uploadService.ProcessDocumentUploadAsync(request.ContentBodyUpload, User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value, request.ContentHeading, request.ContentSubHeading, request.ImageUpload));

    }


    [HttpPost("upload-bulk")]
    [Authorize(Roles = "HR_ADMIN")]
    public async Task<IActionResult> UploadBulkDocuments([FromForm] List<DocumentUploadRequest> requests)
    {

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var files = requests.Where(r => r.ContentBodyUpload != null).Select(r => r.ContentBodyUpload!).ToList();
        var contentHeading = requests.FirstOrDefault()?.ContentHeading ?? string.Empty;
        var contentSubHeading = requests.FirstOrDefault()?.ContentSubHeading ?? string.Empty;
        var responses = await _uploadService.ProcessDocumentsBulkAsync(files, userId!, contentHeading, contentSubHeading);

        return string.IsNullOrEmpty(userId) ? Unauthorized(ApiResponse<IEnumerable<DocumentUploadResponse>>.Failure("User not authenticated")) :
               Ok(ApiResponse<IEnumerable<DocumentUploadResponse>>.Success(responses));
    }


    [HttpGet("all")]
    [Authorize]
    public async Task<IActionResult> GetAllDocuments([FromQuery] PaginationRequest request)
    {
        _logger.LogInformation("Get all documents request received. Page: {PageNumber}, Size: {PageSize}", request.PageNumber, request.PageSize);

        var response = await _uploadService.GetAllDocumentsPaginatedAsync(request);

        return Ok(ApiResponse<PaginatedResponse<DocumentUploadResponse>>.Success(response));
    }



    [HttpGet("get-document/{id}")]
    [Authorize]
    public async Task<IActionResult> GetDocumentById(int id)
    {
        var document = await _uploadService.GetDocumentByIdAsync(id);
        
          var response = new DocumentUploadResponse
        {
            BodyContentFileType = document!.FileName,
            BodyFilePath = document.FilePath,
            BodyContent = document.Content,
            ImageFilePath = document.ImageFilePath,
            ImageFileType = document.ImageFileType,
            ContentHeading = document.ContentHeading,
            ContentSubHeading = document.ContentSubHeading
        };
        
        return document == null ?
            NotFound(ApiResponse<DocumentUploadResponse>.Failure("Document not found.")) :
            Ok(ApiResponse<DocumentUploadResponse>.Success(response));
    }

}
      
