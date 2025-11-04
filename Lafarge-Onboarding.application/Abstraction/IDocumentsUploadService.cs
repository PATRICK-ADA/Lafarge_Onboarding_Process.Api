namespace Lafarge_Onboarding.application.Abstraction;

public interface IDocumentsUploadService
{
    Task<OnboardingDocument> UploadDocumentAsync(IFormFile file, string uploadedBy, string? contentHeading, string? contentSubHeading, IFormFile? imageFile = null);
    Task<string> ExtractTextFromDocumentAsync(string filePath);
    Task<IEnumerable<OnboardingDocument>> GetAllDocumentsAsync();
    Task<PaginatedResponse<DocumentUploadResponse>> GetAllDocumentsPaginatedAsync(PaginationRequest request);
    Task<OnboardingDocument?> GetDocumentByIdAsync(int id);
    Task<DocumentUploadResponse> ProcessDocumentUploadAsync(IFormFile file, string userId, string? contentHeading, string? contentSubHeading, IFormFile? imageFile = null);
    Task<IEnumerable<DocumentUploadResponse>> ProcessDocumentsBulkAsync(IEnumerable<IFormFile> files, string userId, string? contentHeading, string? contentSubHeading);
}