
namespace Lafarge_Onboarding.application.Services;

public class DocumentsUploadService : IDocumentsUploadService
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
            Message = "Document retrieved successfully.",
            DocumentId = doc.Id,
            FileName = doc.FileName,
            FilePath = doc.FilePath,
            ContentLength = doc.Content.Length,
            Content = doc.Content
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

    public async Task<DocumentUploadResponse> ProcessDocumentUploadAsync(IFormFile file, string documentType, string userId)
    {
        _logger.LogInformation("Document upload request received for type: {DocumentType}", documentType);

        var document = await UploadDocumentAsync(file, documentType, userId);

        _logger.LogInformation("Document upload completed successfully. ID: {DocumentId}, Type: {DocumentType}", document.Id, documentType);

        var response = new DocumentUploadResponse
        {
            Message = $"File uploaded successfully for {documentType}.",
            DocumentId = document.Id,
            FileName = document.FileName,
            FilePath = document.FilePath,
            ContentLength = document.Content.Length,
            Content = document.Content
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

    public async Task<OnboardingDocument> UploadDocumentAsync(IFormFile file, string documentType, string uploadedBy)
    {
        _logger.LogInformation("Starting document upload process for type: {DocumentType}, uploaded by: {UploadedBy}", documentType, uploadedBy);

        if (file == null || file.Length == 0)
        {
            _logger.LogWarning("Upload attempt with null or empty file for document type: {DocumentType}", documentType);
            throw new ArgumentException("No file uploaded.");
        }

    
        if (string.IsNullOrWhiteSpace(documentType))
        {
            _logger.LogWarning("Document type is required");
            throw new ArgumentException("Document type is required.");
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

        
        var docTypePath = Path.Combine(uploadsPath, documentType);
        if (!Directory.Exists(docTypePath))
        {
            Directory.CreateDirectory(docTypePath);
            _logger.LogInformation("Created document type directory: {DocTypePath}", docTypePath);
        }

        // Generate unique filename to avoid conflicts
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
        var filePath = Path.Combine(docTypePath, uniqueFileName);

        
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

        // Create document entity
        var document = new OnboardingDocument
        {
            DocumentType = documentType,
            FileName = uniqueFileName,
            FilePath = filePath,
            Content = content,
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

    public async Task<IEnumerable<DocumentUploadResponse>> ProcessDocumentsBulkAsync(IEnumerable<IFormFile> files, string documentType, string userId)
    {
        _logger.LogInformation("Bulk document upload request received for type: {DocumentType}, file count: {FileCount}", documentType, files.Count());

        var maxDegree = Math.Max(1, Environment.ProcessorCount - 1);
        using var semaphore = new SemaphoreSlim(maxDegree);

        var tasks = files.Select(async file =>
        {
            await semaphore.WaitAsync();
            try
            {
                return await ProcessDocumentUploadAsync(file, documentType, userId);
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