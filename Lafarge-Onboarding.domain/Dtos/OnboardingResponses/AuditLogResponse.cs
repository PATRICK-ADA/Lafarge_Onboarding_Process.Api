namespace Lafarge_Onboarding.domain.Dtos.OnboardingResponses;

public sealed record AuditLogResponse
{
    public required int Id { get; init; }
    public required DateTime Timestamp { get; init; }
    public string? UserId { get; init; }
    public string? UserName { get; init; }
    public string? UserEmail { get; init; }
    public string? UserRole { get; init; }
    public required string Action { get; init; }
    public string? Description { get; init; }
    public required string ResourceType { get; init; }
    public string? ResourceId { get; init; }
    public string? HttpMethod { get; init; }
    public string? Url { get; init; }
    public int? StatusCode { get; init; }
    public string? IpAddress { get; init; }
    public required string Status { get; init; }
    public string? OldValues { get; init; }
    public string? NewValues { get; init; }
    public string? AdditionalData { get; init; }
    public required DateTime CreatedAt { get; init; }
}