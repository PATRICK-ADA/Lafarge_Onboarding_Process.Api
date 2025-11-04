public sealed record UploadBulkUsersRequests
{
    public List<CreateUserRequest> Users { get; init; } = new();
}