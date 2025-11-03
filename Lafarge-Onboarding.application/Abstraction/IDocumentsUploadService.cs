using Lafarge_Onboarding.domain.Entities;
using Lafarge_Onboarding.domain.OnboardingRequests;
using Lafarge_Onboarding.domain.OnboardingResponses;
using Microsoft.AspNetCore.Http;

namespace Lafarge_Onboarding.application.Abstraction;

public interface IDocumentsUploadService
{
    Task<OnboardingDocument> UploadDocumentAsync(IFormFile file, string documentType, string uploadedBy);
    Task<string> ExtractTextFromDocumentAsync(string filePath);
    Task<IEnumerable<OnboardingDocument>> GetAllDocumentsAsync();
    Task<PaginatedResponse<DocumentUploadResponse>> GetAllDocumentsPaginatedAsync(PaginationRequest request);
    Task<OnboardingDocument?> GetDocumentByIdAsync(int id);
    Task<DocumentUploadResponse> ProcessDocumentUploadAsync(IFormFile file, string documentType, string userId);
    Task<IEnumerable<DocumentUploadResponse>> ProcessDocumentsBulkAsync(IEnumerable<IFormFile> files, string documentType, string userId);
}