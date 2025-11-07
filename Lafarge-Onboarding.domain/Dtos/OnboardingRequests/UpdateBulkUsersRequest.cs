using System.ComponentModel.DataAnnotations;

public sealed record UpdateBulkUsersRequest
{
    [Required(ErrorMessage = "Users is required")]
    public required List<UpdateUserItem> Users { get; init; }
}