namespace Lafarge_Onboarding.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class AuditLogsController : ControllerBase
{
    private readonly IAuditService _auditService;

    public AuditLogsController(IAuditService auditService)
    {
        _auditService = auditService;
    }

    [HttpGet("get-audit-logs")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<AuditLogResponse>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] string? userId,
        [FromQuery] string? userName,
        [FromQuery] string? userEmail,
        [FromQuery] string? userRole,
        [FromQuery] string? action,
        [FromQuery] string? resourceType,
        [FromQuery] string? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = true)
    {
        var filter = new AuditLogFilterRequest
        {
            StartDate = startDate,
            EndDate = endDate,
            UserId = userId,
            UserName = userName,
            UserEmail = userEmail,
            Action = action,
            ResourceType = resourceType,
            Status = status,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _auditService.GetAuditLogsAsync(filter, sortBy, sortDescending);
        return Ok(ApiResponse<PaginatedResponse<AuditLogResponse>>.Success(result));
    }
}