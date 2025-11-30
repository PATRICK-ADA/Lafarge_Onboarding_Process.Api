namespace Lafarge_Onboarding.application.Abstraction;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog auditLog);
    Task<(IEnumerable<AuditLogResponse> Items, int TotalCount)> GetFilteredAsync(
        AuditLogFilterRequest filter,
        string? sortBy = null,
        bool sortDescending = true);
}