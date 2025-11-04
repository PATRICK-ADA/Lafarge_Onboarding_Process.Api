namespace Lafarge_Onboarding.application.Abstraction;

public interface IDocumentsUploadRepository
{
    Task AddAsync(OnboardingDocument document);
    Task<OnboardingDocument?> GetByIdAsync(int id);
    Task<IEnumerable<OnboardingDocument>> GetAllAsync();
    Task<(IEnumerable<OnboardingDocument> Items, int TotalCount)> GetAllPaginatedAsync(PaginationRequest request);
}