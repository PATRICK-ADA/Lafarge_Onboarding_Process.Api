namespace Lafarge_Onboarding.application.Services;

public sealed class AuditService : IAuditService
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuditService> _logger;

    public AuditService(
        IAuditLogRepository auditLogRepository,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuditService> logger)
    {
        _auditLogRepository = auditLogRepository;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task LogAuditEventAsync(
        string action,
        string resourceType,
        string? resourceId = null,
        string? description = null,
        string status = "Success",
        string? oldValues = null,
        string? newValues = null,
        string? additionalData = null)
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var user = httpContext?.User;

            // Capture user context
            var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userEmail = user?.FindFirst(ClaimTypes.Email)?.Value;
            var userName = user != null
                ? $"{user.FindFirst(ClaimTypes.GivenName)?.Value} {user.FindFirst(ClaimTypes.Surname)?.Value}".Trim()
                : null;
            var userRole = user?.FindFirst(ClaimTypes.Role)?.Value;

            // Capture HTTP details
            var httpMethod = httpContext?.Request.Method;
            var url = httpContext != null
                ? $"{httpContext.Request.Path}{httpContext.Request.QueryString}"
                : null;
            var ipAddress = httpContext?.Connection.RemoteIpAddress?.ToString();
            var statusCode = httpContext?.Response.StatusCode;

            var auditLog = new AuditLog
            {
                Timestamp = DateTime.UtcNow,
                UserId = userId,
                UserName = userName,
                UserEmail = userEmail,
                UserRole = userRole,
                Action = action,
                Description = description,
                ResourceType = resourceType,
                ResourceId = resourceId,
                HttpMethod = httpMethod,
                Url = url,
                StatusCode = statusCode,
                IpAddress = ipAddress,
                Status = status,
                OldValues = oldValues,
                NewValues = newValues,
                AdditionalData = additionalData,
                CreatedAt = DateTime.UtcNow
            };

            await _auditLogRepository.AddAsync(auditLog);

            _logger.LogInformation("Audit event logged: {Action} on {ResourceType} {ResourceId} by {UserEmail}",
                action, resourceType, resourceId, userEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log audit event: {Action} on {ResourceType}",
                action, resourceType);
           
        }
    }

    public async Task<PaginatedResponse<AuditLogResponse>> GetAuditLogsAsync(
        AuditLogFilterRequest filter,
        string? sortBy = null,
        bool sortDescending = true)
    {
        _logger.LogInformation("Retrieving audit logs with filter");

        var (items, totalCount) = await _auditLogRepository.GetFilteredAsync(filter, sortBy, sortDescending);

        var response = new PaginatedResponse<AuditLogResponse>
        {
            Content = items,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalCount = totalCount
        };

        _logger.LogInformation("Retrieved {Count} audit logs out of {TotalCount}",
            items.Count(), totalCount);

        return response;
    }
}