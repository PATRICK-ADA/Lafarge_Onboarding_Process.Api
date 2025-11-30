namespace Lafarge_Onboarding.domain.Entities;

public class AuditLog
{
    public int Id { get; set; }

    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public string? UserId { get; set; }

    [MaxLength(100)]
    public string? UserName { get; set; }

    [MaxLength(255)]
    public string? UserEmail { get; set; }

    [MaxLength(100)]
    public string? UserRole { get; set; }

    [Required]
    [MaxLength(50)]
    public string Action { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(100)]
    public string ResourceType { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? ResourceId { get; set; }

    [MaxLength(10)]
    public string? HttpMethod { get; set; }

    [MaxLength(500)]
    public string? Url { get; set; }

    public int? StatusCode { get; set; }

    [MaxLength(45)]
    public string? IpAddress { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Success";

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string? AdditionalData { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}