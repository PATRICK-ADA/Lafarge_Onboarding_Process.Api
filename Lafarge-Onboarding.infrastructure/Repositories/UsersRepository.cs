
namespace Lafarge_Onboarding.infrastructure.Repositories;

public sealed class UsersRepository : IUsersRepository
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

    public async Task<bool> UpdateUserAsync(string id, UpdateUserRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(request.Name))
        {
            var nameParts = request.Name.Split(' ', 2);
            user.FirstName = nameParts[0];
            user.LastName = nameParts.Length > 1 ? nameParts[1] : "";
        }

        if (!string.IsNullOrEmpty(request.Email))
        {
            user.Email = request.Email;
            user.UserName = request.Email;
        }

        if (!string.IsNullOrEmpty(request.PhoneNumber))
        {
            user.PhoneNumber = request.PhoneNumber;
        }

        if (!string.IsNullOrEmpty(request.Role))
        {
            user.Role = request.Role;
        }


        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> UpdateBulkUsersAsync(IEnumerable<UpdateUserItem> users)
    {
        var successCount = 0;
        foreach (var userItem in users)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userItem.Id);
            if (user == null)
            {
                continue; // Skip if user not found, validation should be done at service level
            }

            if (!string.IsNullOrEmpty(userItem.Name))
            {
                var nameParts = userItem.Name.Split(' ', 2);
                user.FirstName = nameParts[0];
                user.LastName = nameParts.Length > 1 ? nameParts[1] : "";
            }

            if (!string.IsNullOrEmpty(userItem.Email))
            {
                user.Email = userItem.Email;
                user.UserName = userItem.Email;
            }

            if (!string.IsNullOrEmpty(userItem.PhoneNumber))
            {
                user.PhoneNumber = userItem.PhoneNumber;
            }

            if (!string.IsNullOrEmpty(userItem.Role))
            {
                user.Role = userItem.Role;
            }


            successCount++;
        }

        await _context.SaveChangesAsync();
        return successCount;
    }

    public async Task<bool> DeleteUserAsync(string id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return false;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> DeleteUsersByRoleAsync(string role)
    {
        var users = await _context.Users.Where(u => u.Role == role).ToListAsync();
        _context.Users.RemoveRange(users);
        await _context.SaveChangesAsync();
        return users.Count;
    }
}