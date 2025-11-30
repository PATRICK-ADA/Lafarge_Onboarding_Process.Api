
namespace Lafarge_Onboarding.infrastructure.Repositories;

public sealed class DocumentsUploadRepository : IDocumentsUploadRepository
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

    public async Task<DocumentUploadResponse?> GetByIdAsync(int id)
    {
        return await _context.OnboardingDocuments
            .Where(d => d.Id == id)
            .AsNoTracking()
            .Select(d => new DocumentUploadResponse
            {
                BodyContentFileType = d.FileName,
                BodyFilePath = d.FilePath,
                BodyContent = d.Content,
                ImageFilePath = d.ImageFilePath,
                ImageFileType = d.ImageFileType,
                ContentHeading = d.ContentHeading,
                ContentSubHeading = d.ContentSubHeading
            })
            .FirstOrDefaultAsync();
    }


    public async Task<IEnumerable<DocumentUploadResponse>> GetAllAsync()
    {
        return await _context.OnboardingDocuments
            .AsNoTracking()
            .Select(d => new DocumentUploadResponse
            {
                BodyContentFileType = d.FileName,
                BodyFilePath = d.FilePath,
                BodyContent = d.Content,
                ImageFilePath = d.ImageFilePath,
                ImageFileType = d.ImageFileType,
                ContentHeading = d.ContentHeading,
                ContentSubHeading = d.ContentSubHeading
            })
            .ToListAsync();
    }

    public async Task<(IEnumerable<DocumentUploadResponse> Items, int TotalCount)> GetAllPaginatedAsync(PaginationRequest request)
    {
        var query = _context.OnboardingDocuments.AsQueryable();

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip(request.Skip)
            .Take(request.PageSize)
            .AsNoTracking()
            .Select(d => new DocumentUploadResponse
            {
                BodyContentFileType = d.FileName,
                BodyFilePath = d.FilePath,
                BodyContent = d.Content,
                ImageFilePath = d.ImageFilePath,
                ImageFileType = d.ImageFileType,
                ContentHeading = d.ContentHeading,
                ContentSubHeading = d.ContentSubHeading
            })
            .ToListAsync();

        return (items, totalCount);
    }
}