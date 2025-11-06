public sealed record CreateUserRequest
{
    public required string Name { get; init; }
    public required string Email { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Role { get; init; }
    public string? Department { get; init; }
}

 