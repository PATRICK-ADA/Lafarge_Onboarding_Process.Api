public sealed record GetUserResponse
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Role { get; init; }
    public string? Department { get; init; }
    public required DateTime CreatedAt { get; init; }
}