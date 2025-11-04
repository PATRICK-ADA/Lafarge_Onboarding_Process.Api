
namespace Lafarge_Onboarding.application.Services;

public sealed class DocumentsUploadService : IDocumentsUploadService
{
    private readonly IDocumentsUploadRepository _repository;
    private readonly ILogger<DocumentsUploadService> _logger;

    public DocumentsUploadService(IDocumentsUploadRepository repository, ILogger<DocumentsUploadService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<OnboardingDocument>> GetAllDocumentsAsync()
    {
        _logger.LogInformation("Retrieving all documents");
        return await _repository.GetAllAsync();
    }

    public async Task<OnboardingDocument?> GetDocumentByIdAsync(int id)
    {
        _logger.LogInformation("Retrieving document by ID: {DocumentId}", id);
        return await _repository.GetByIdAsync(id);
    }

    public async Task<PaginatedResponse<DocumentUploadResponse>> GetAllDocumentsPaginatedAsync(PaginationRequest request)
    {
        _logger.LogInformation("Retrieving paginated documents. Page: {PageNumber}, Size: {PageSize}", request.PageNumber, request.PageSize);

        var (documents, totalCount) = await _repository.GetAllPaginatedAsync(request);

        var data = documents.Select(doc => new DocumentUploadResponse
        {
            BodyContentFileType = doc.FileName,
            BodyFilePath = doc.FilePath,
            BodyContent = doc.Content,
            ImageFilePath = doc.ImageFilePath,
            ImageFileType = doc.ImageFileType,
            ContentHeading = doc.ContentHeading,
            ContentSubHeading = doc.ContentSubHeading
        });

        var response = new PaginatedResponse<DocumentUploadResponse>
        {
            Data = data,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };

        _logger.LogInformation("Retrieved {DocumentCount} documents out of {TotalCount}", documents.Count(), totalCount);
        return response;
    }

    public async Task<DocumentUploadResponse> ProcessDocumentUploadAsync(IFormFile file, string userId, string? contentHeading, string? contentSubHeading, IFormFile? imageFile = null)
    {
        _logger.LogInformation("Document upload request received");

        var document = await UploadDocumentAsync(file, userId, contentHeading, contentSubHeading, imageFile);

        _logger.LogInformation("Document upload completed successfully. ID: {DocumentId}", document.Id);

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

        return response;
    }

    // Define allowed document types
    private readonly string[] _allowedDocumentTypes = {
        "welcome-message-CEO",
        "welcome-message-HR",
        "about-lafarge",
        "welcome-to-nigeria",
        "key-contacts",
        "etiquette-&&-faux-pas",
        "gallery"
    };

    public async Task<OnboardingDocument> UploadDocumentAsync(IFormFile file, string uploadedBy, string? contentHeading, string? contentSubHeading, IFormFile? imageFile = null)
    {
        _logger.LogInformation("Starting document upload process, uploaded by: {UploadedBy}", uploadedBy);

        if (file == null || file.Length == 0)
        {
            _logger.LogWarning("Upload attempt with null or empty file");
            throw new ArgumentException("No file uploaded.");
        }


        const long maxFileSize = 10 * 1024 * 1024; // 10MB
        if (file.Length > maxFileSize)
        {
            _logger.LogWarning("File size validation failed. File: {FileName}, Size: {FileSize} bytes", file.FileName, file.Length);
            throw new ArgumentException("File size exceeds the maximum allowed size of 10MB.");
        }


        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".txt", ".jpg", ".jpeg", ".png", ".gif" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(fileExtension))
        {
            _logger.LogWarning("File type validation failed. File: {FileName}, Extension: {Extension}", file.FileName, fileExtension);
            throw new ArgumentException("Invalid file type. Allowed types: PDF, DOC, DOCX, TXT, JPG, JPEG, PNG, GIF.");
        }

        // Ensure uploads directory exists
        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        if (!Directory.Exists(uploadsPath))
        {
            Directory.CreateDirectory(uploadsPath);
            _logger.LogInformation("Created uploads directory: {UploadsPath}", uploadsPath);
        }

        // Generate unique filename to avoid conflicts
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
        var filePath = Path.Combine(uploadsPath, uniqueFileName);


        _logger.LogInformation("Saving file to: {FilePath}", filePath);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        _logger.LogInformation("File saved successfully. Size: {FileSize} bytes", file.Length);

        // Extract text content from the document
        _logger.LogInformation("Starting text extraction for file: {FileName}", uniqueFileName);
        var content = await ExtractTextFromDocumentAsync(filePath);
        _logger.LogInformation("Text extraction completed. Content length: {ContentLength} characters", content.Length);

        // Handle image upload if provided
        string? imageFilePath = null;
        string? imageFileType = null;
        if (imageFile != null && imageFile.Length > 0)
        {
            var imageExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            if (!allowedImageExtensions.Contains(imageExtension))
            {
                _logger.LogWarning("Invalid image file type: {Extension}", imageExtension);
                throw new ArgumentException("Invalid image file type. Allowed types: JPG, JPEG, PNG, GIF.");
            }

            var uniqueImageFileName = $"{Guid.NewGuid()}{imageExtension}";
            imageFilePath = Path.Combine(uploadsPath, uniqueImageFileName);
            imageFileType = imageExtension;

            _logger.LogInformation("Saving image to: {ImageFilePath}", imageFilePath);
            using (var stream = new FileStream(imageFilePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            _logger.LogInformation("Image saved successfully. Size: {FileSize} bytes", imageFile.Length);
        }

        // Create document entity
        var document = new OnboardingDocument
        {
            FileName = uniqueFileName,
            FilePath = filePath,
            Content = content,
            ContentHeading = contentHeading,
            ContentSubHeading = contentSubHeading,
            ImageFilePath = imageFilePath,
            ImageFileType = imageFileType,
            UploadedAt = DateTime.UtcNow,
            UploadedBy = uploadedBy
        };

        // Save to database
        _logger.LogInformation("Saving document metadata to database");
        await _repository.AddAsync(document);
        _logger.LogInformation("Document upload completed successfully. Document ID: {DocumentId}", document.Id);

        return document;
    }

    public async Task<string> ExtractTextFromDocumentAsync(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();

        return extension switch
        {
            ".txt" => await ExtractTextFromTxtAsync(filePath),
            ".docx" => await ExtractTextFromDocxAsync(filePath),
            ".doc" => await ExtractTextFromDocAsync(filePath),
            ".pdf" => await ExtractTextFromPdfAsync(filePath),
            _ => string.Empty // For images and other files, return empty or handle differently
        };
    }

    private async Task<string> ExtractTextFromTxtAsync(string filePath)
    {
        return await File.ReadAllTextAsync(filePath);
    }

    private async Task<string> ExtractTextFromDocxAsync(string filePath)
    {
        return await Task.Run(() =>
        {
            _logger.LogInformation("Starting DOCX text extraction for file: {FilePath}", filePath);
            try
            {
                using (var document = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(filePath, false))
                {
                    var body = document.MainDocumentPart?.Document.Body;
                    if (body == null)
                    {
                        _logger.LogWarning("DOCX document has no body content: {FilePath}", filePath);
                        return string.Empty;
                    }

                    var text = body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>()
                        .Select(t => t.Text)
                        .Aggregate((current, next) => current + " " + next);

                    _logger.LogInformation("DOCX text extraction completed successfully. Characters extracted: {CharCount}", text?.Length ?? 0);
                    return text ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting DOCX content from file: {FilePath}", filePath);
                return $"Error extracting DOCX content: {ex.Message}";
            }
        });
    }

    private async Task<string> ExtractTextFromDocAsync(string filePath)
    {
        return await Task.Run(() =>
        {
            _logger.LogInformation("DOC file extraction attempted (not implemented): {FilePath}", filePath);
            // DOC files are older format - for simplicity, we'll treat them as binary
            // In production, you might need additional libraries or conversion
            _logger.LogWarning("DOC file format extraction requires additional processing. File: {FileName}", Path.GetFileName(filePath));
            return "DOC file format extraction requires additional processing. File: " + Path.GetFileName(filePath);
        });
    }

    private async Task<string> ExtractTextFromPdfAsync(string filePath)
    {
        return await Task.Run(() =>
        {
            _logger.LogInformation("Starting PDF text extraction for file: {FilePath}", filePath);
            try
            {
                using (var pdf = UglyToad.PdfPig.PdfDocument.Open(filePath))
                {
                    var pages = pdf.GetPages().ToList();
                    _logger.LogInformation("PDF has {PageCount} pages", pages.Count);

                    var text = string.Join(" ", pages.Select(page => page.Text));
                    _logger.LogInformation("PDF text extraction completed successfully. Characters extracted: {CharCount}", text.Length);
                    return text;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting PDF content from file: {FilePath}", filePath);
                return $"Error extracting PDF content: {ex.Message}";
            }
        });
    }

    public async Task<IEnumerable<DocumentUploadResponse>> ProcessDocumentsBulkAsync(IEnumerable<IFormFile> files, string userId, string? contentHeading, string? contentSubHeading)
    {
        _logger.LogInformation("Bulk document upload request received, file count: {FileCount}", files.Count());

        var maxDegree = Math.Max(1, Environment.ProcessorCount - 1);
        using var semaphore = new SemaphoreSlim(maxDegree);

        var tasks = files.Select(async file =>
        {
            await semaphore.WaitAsync();
            try
            {
                return await ProcessDocumentUploadAsync(file, userId, contentHeading, contentSubHeading);
            }
            finally
            {
                semaphore.Release();
            }
        }).ToList();

        var responses = await Task.WhenAll(tasks);
        _logger.LogInformation("Bulk document upload completed. Processed {DocumentCount} documents", responses.Length);

        return responses;
    }
}