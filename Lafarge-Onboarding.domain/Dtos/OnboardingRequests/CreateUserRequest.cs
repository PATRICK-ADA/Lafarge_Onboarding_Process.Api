using System.ComponentModel.DataAnnotations;

public sealed record CreateUserRequest
{
    [Required(ErrorMessage = "Name is required")]
    public required string Name { get; init; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public required string Email { get; init; }

    public string? PhoneNumber { get; init; }

    [Required(ErrorMessage = "Role is required")]
    public required string Role { get; init; }

    [Required(ErrorMessage = "Department is required")]
    public required string Department { get; init; }
}

 