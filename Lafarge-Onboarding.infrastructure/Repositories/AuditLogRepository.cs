namespace Lafarge_Onboarding.infrastructure.Repositories;

public sealed class AuditLogRepository : IAuditLogRepository
{
    private readonly ApplicationDbContext _context;

    public AuditLogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AuditLog auditLog)
    {
        await _context.AuditLogs.AddAsync(auditLog);
        await _context.SaveChangesAsync();
    }

    public async Task<(IEnumerable<AuditLogResponse> Items, int TotalCount)> GetFilteredAsync(
        AuditLogFilterRequest filter,
        string? sortBy = null,
        bool sortDescending = true)
    {
        var query = _context.AuditLogs.AsQueryable();

        // Apply filters
        if (filter.StartDate.HasValue)
        {
            query = query.Where(x => x.Timestamp >= filter.StartDate.Value);
        }

        if (filter.EndDate.HasValue)
        {
            query = query.Where(x => x.Timestamp <= filter.EndDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.UserId))
        {
            query = query.Where(x => x.UserId == filter.UserId);
        }

        if (!string.IsNullOrWhiteSpace(filter.UserName))
        {
            query = query.Where(x => x.UserName!.Contains(filter.UserName));
        }

        if (!string.IsNullOrWhiteSpace(filter.UserEmail))
        {
            query = query.Where(x => x.UserEmail!.Contains(filter.UserEmail));
        }

        if (!string.IsNullOrWhiteSpace(filter.UserRole))
        {
            query = query.Where(x => x.UserRole!.Contains(filter.UserRole));
        }

        if (!string.IsNullOrWhiteSpace(filter.Action))
        {
            query = query.Where(x => x.Action.Contains(filter.Action));
        }
if (!string.IsNullOrWhiteSpace(filter.ResourceType))
{
    query = query.Where(x => x.ResourceType.Contains(filter.ResourceType));
}
if (!string.IsNullOrWhiteSpace(filter.Status))
{
    query = query.Where(x => x.Status.Contains(filter.Status));
}
        var totalCount = await query.CountAsync();

        // Apply sorting
        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            query = sortBy.ToLower() switch
            {
                "timestamp" => sortDescending
                    ? query.OrderByDescending(x => x.Timestamp)
                    : query.OrderBy(x => x.Timestamp),
                "userid" => sortDescending
                    ? query.OrderByDescending(x => x.UserId)
                    : query.OrderBy(x => x.UserId),
                "username" => sortDescending
                    ? query.OrderByDescending(x => x.UserName)
                    : query.OrderBy(x => x.UserName),
                "useremail" => sortDescending
                    ? query.OrderByDescending(x => x.UserEmail)
                    : query.OrderBy(x => x.UserEmail),
                "userrole" => sortDescending
                    ? query.OrderByDescending(x => x.UserRole)
                    : query.OrderBy(x => x.UserRole),
                "action" => sortDescending
                    ? query.OrderByDescending(x => x.Action)
                    : query.OrderBy(x => x.Action),
                "resourcetype" => sortDescending
                    ? query.OrderByDescending(x => x.ResourceType)
                    : query.OrderBy(x => x.ResourceType),
                "resourceid" => sortDescending
                    ? query.OrderByDescending(x => x.ResourceId)
                    : query.OrderBy(x => x.ResourceId),
                "status" => sortDescending
                    ? query.OrderByDescending(x => x.Status)
                    : query.OrderBy(x => x.Status),
                "httpmethod" => sortDescending
                    ? query.OrderByDescending(x => x.HttpMethod)
                    : query.OrderBy(x => x.HttpMethod),
                "statuscode" => sortDescending
                    ? query.OrderByDescending(x => x.StatusCode)
                    : query.OrderBy(x => x.StatusCode),
                "ipaddress" => sortDescending
                    ? query.OrderByDescending(x => x.IpAddress)
                    : query.OrderBy(x => x.IpAddress),
                "createdat" => sortDescending
                    ? query.OrderByDescending(x => x.CreatedAt)
                    : query.OrderBy(x => x.CreatedAt),
                _ => sortDescending
                    ? query.OrderByDescending(x => x.Timestamp)
                    : query.OrderBy(x => x.Timestamp)
            };
        }
        else
        {
            // Default sort by timestamp descending
            query = query.OrderByDescending(x => x.Timestamp);
        }

        
        var items = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .AsNoTracking()
            .Select(x => new AuditLogResponse
            {
                Id = x.Id,
                Timestamp = x.Timestamp,
                UserId = x.UserId,
                UserName = x.UserName,
                UserEmail = x.UserEmail,
                UserRole = x.UserRole,
                Action = x.Action,
                Description = x.Description,
                ResourceType = x.ResourceType,
                ResourceId = x.ResourceId,
                HttpMethod = x.HttpMethod,
                Url = x.Url,
                StatusCode = x.StatusCode,
                IpAddress = x.IpAddress,
                Status = x.Status,
                OldValues = x.OldValues,
                NewValues = x.NewValues,
                AdditionalData = x.AdditionalData,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return (items, totalCount);
    }
}