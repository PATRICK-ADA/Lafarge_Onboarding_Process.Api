namespace Lafarge_Onboarding.domain.Dtos.OnboardingResponses;

public sealed record GalleryResponse
{
    public required int Id { get; init; }
    public required string ImageName { get; init; }
    public required string ImageBase64 { get; init; }
    public required string ImageType { get; init; }
    public required DateTime UploadedAt { get; init; }
    public string? UploadedBy { get; init; }
}