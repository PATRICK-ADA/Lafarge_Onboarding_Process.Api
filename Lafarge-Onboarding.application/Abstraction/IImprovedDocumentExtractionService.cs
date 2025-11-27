namespace Lafarge_Onboarding.application.Abstraction;

public interface IImprovedDocumentExtractionService
{
    Task<Dictionary<string, string>> ExtractStructuredSectionsAsync(IFormFile file);
}
