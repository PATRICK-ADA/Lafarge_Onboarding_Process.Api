namespace Lafarge_Onboarding.infrastructure.Repositories;

public sealed class GalleryRepository : IGalleryRepository
{
    private readonly ApplicationDbContext _context;

    public GalleryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Gallery gallery)
    {
        _context.Galleries.Add(gallery);
        await _context.SaveChangesAsync();
    }

    public async Task<List<GalleryResponse>> GetByImageTypeAsync(string imageType)
    {
        return await _context.Galleries
            .Where(g => g.ImageType == imageType)
            .OrderByDescending(g => g.UploadedAt)
            .AsNoTracking()
            .Select(g => new GalleryResponse
            {
                Id = g.Id,
                ImageName = g.ImageName,
                ImageBase64 = g.ImageBase64,
                ImageType = g.ImageType,
                UploadedAt = g.UploadedAt,
                UploadedBy = g.UploadedBy
            })
            .ToListAsync();
    }

    public async Task DeleteByImageTypeAsync(string imageType)
    {
        var images = await _context.Galleries
            .Where(g => g.ImageType == imageType)
            .ToListAsync();
        
        _context.Galleries.RemoveRange(images);
        await _context.SaveChangesAsync();
    }

    public async Task<GalleryResponse?> GetByIdAsync(int id)
    {
        return await _context.Galleries
            .Where(g => g.Id == id)
            .AsNoTracking()
            .Select(g => new GalleryResponse
            {
                Id = g.Id,
                ImageName = g.ImageName,
                ImageBase64 = g.ImageBase64,
                ImageType = g.ImageType,
                UploadedAt = g.UploadedAt,
                UploadedBy = g.UploadedBy
            })
            .FirstOrDefaultAsync();
    }

    public async Task DeleteByIdAsync(int id)
    {
        var image = await _context.Galleries.FindAsync(id);
        if (image != null)
        {
            _context.Galleries.Remove(image);
            await _context.SaveChangesAsync();
        }
    }
}