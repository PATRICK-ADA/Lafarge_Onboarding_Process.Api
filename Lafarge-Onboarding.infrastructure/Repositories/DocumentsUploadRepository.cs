
namespace Lafarge_Onboarding.infrastructure.Repositories;

public class DocumentsUploadRepository : IDocumentsUploadRepository
{
    private readonly ApplicationDbContext _context;

    public DocumentsUploadRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(OnboardingDocument document)
    {
        await _context.OnboardingDocuments.AddAsync(document);
        await _context.SaveChangesAsync();
    }

    public async Task<OnboardingDocument?> GetByIdAsync(int id)
    {
        return await _context.OnboardingDocuments.FindAsync(id);
    }

    public async Task<IEnumerable<OnboardingDocument>> GetByDocumentTypeAsync(string documentType)
    {
        return await _context.OnboardingDocuments
            .Where(d => d.DocumentType == documentType)
            .ToListAsync();
    }

    public async Task<IEnumerable<OnboardingDocument>> GetAllAsync()
    {
        return await _context.OnboardingDocuments.ToListAsync();
    }

    public async Task<(IEnumerable<OnboardingDocument> Items, int TotalCount)> GetAllPaginatedAsync(PaginationRequest request)
    {
        var query = _context.OnboardingDocuments.AsQueryable();

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip(request.Skip)
            .Take(request.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}