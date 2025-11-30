namespace Lafarge_Onboarding.application.Abstraction;

public interface IAuditService
{
    /// <summary>
    /// Logs an audit event with automatic capture of user context, HTTP details, and operation results.
    /// </summary>
    /// <param name="action">The action performed (e.g., "CREATE", "UPDATE", "DELETE").</param>
    /// <param name="resourceType">The type of resource being audited (e.g., "User", "Document").</param>
    /// <param name="resourceId">The ID of the specific resource (optional).</param>
    /// <param name="description">A description of the audit event (optional).</param>
    /// <param name="status">The status of the operation ("Success" or "Failed").</param>
    /// <param name="oldValues">The old values before the operation (optional, JSON string).</param>
    /// <param name="newValues">The new values after the operation (optional, JSON string).</param>
    /// <param name="additionalData">Any additional data related to the audit event (optional, JSON string).</param>
    Task LogAuditEventAsync(
        string action,
        string resourceType,
        string? resourceId = null,
        string? description = null,
        string status = "Success",
        string? oldValues = null,
        string? newValues = null,
        string? additionalData = null);

    /// <summary>
    /// Retrieves audit logs with filtering and pagination.
    /// </summary>
    /// <param name="filter">The filter criteria for retrieving audit logs.</param>
    /// <param name="sortBy">The field to sort by (optional, defaults to timestamp).</param>
    /// <param name="sortDescending">Whether to sort in descending order (default: true).</param>
    /// <returns>A paginated response containing the filtered audit logs.</returns>
    Task<PaginatedResponse<AuditLogResponse>> GetAuditLogsAsync(
        AuditLogFilterRequest filter,
        string? sortBy = null,
        bool sortDescending = true);
}