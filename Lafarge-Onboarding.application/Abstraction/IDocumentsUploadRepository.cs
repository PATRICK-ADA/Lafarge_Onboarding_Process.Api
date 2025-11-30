namespace Lafarge_Onboarding.application.Abstraction;

public interface IDocumentsUploadRepository
{
    Task AddAsync(OnboardingDocument document);
    Task<DocumentUploadResponse?> GetByIdAsync(int id);
    Task<IEnumerable<DocumentUploadResponse>> GetAllAsync();
    Task<(IEnumerable<DocumentUploadResponse> Items, int TotalCount)> GetAllPaginatedAsync(PaginationRequest request);
}