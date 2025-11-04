
namespace Lafarge_Onboarding.infrastructure.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly ApplicationDbContext _context;

    public UsersRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Users> Users, int TotalCount)> GetUsersAsync(PaginationRequest pagination)
    {
        var query = _context.Users.AsQueryable();
        var totalCount = await query.CountAsync();
        var users = await query
            .Skip(pagination.Skip)
            .Take(pagination.PageSize)
            .ToListAsync();

        return (users, totalCount);
    }

    public async Task<int> UploadBulkUsersAsync(IEnumerable<Users> users)
    {
        await _context.Users.AddRangeAsync(users);
        var count = await _context.SaveChangesAsync();
        return count;
    }

    public async Task<(IEnumerable<Users> Users, int TotalCount)> GetUsersByRoleAsync(string role, PaginationRequest pagination)
    {
        var query = _context.Users.Where(u => u.Role == role);
        var totalCount = await query.CountAsync();
        var users = await query
            .Skip(pagination.Skip)
            .Take(pagination.PageSize)
            .ToListAsync();

        return (users, totalCount);
    }

    public async Task<(IEnumerable<Users> Users, int TotalCount)> GetUsersByNameAsync(string name, PaginationRequest pagination)
    {
        var query = _context.Users.Where(u =>
            (u.FirstName + " " + u.LastName).Contains(name) ||
            u.FirstName.Contains(name) ||
            u.LastName.Contains(name));
        var totalCount = await query.CountAsync();
        var users = await query
            .Skip(pagination.Skip)
            .Take(pagination.PageSize)
            .ToListAsync();

        return (users, totalCount);
    }

    public async Task<Users?> GetUserByIdAsync(string id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }
}