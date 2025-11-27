namespace Lafarge_Onboarding.application.Services;

public sealed class ImprovedDocumentExtractionService : IImprovedDocumentExtractionService
{
    private readonly ILogger<ImprovedDocumentExtractionService> _logger;

    public ImprovedDocumentExtractionService(ILogger<ImprovedDocumentExtractionService> logger)
    {
        _logger = logger;
    }

    public async Task<Dictionary<string, string>> ExtractStructuredSectionsAsync(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        
        return extension switch
        {
            ".docx" => await ExtractFromDocxAsync(file),
            ".pdf" => await ExtractFromPdfAsync(file),
            _ => new Dictionary<string, string>()
        };
    }

    private async Task<Dictionary<string, string>> ExtractFromDocxAsync(IFormFile file)
    {
        return await Task.Run(() =>
        {
            try
            {
                using var stream = file.OpenReadStream();
                using var doc = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(stream, false);
                
                var body = doc.MainDocumentPart?.Document.Body;
                if (body == null)
                    return new Dictionary<string, string>();

                var allText = new StringBuilder();
                foreach (var para in body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Paragraph>())
                {
                    var text = string.Join("", para.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>()
                        .Select(t => t.Text)).Trim();
                    
                    if (!string.IsNullOrWhiteSpace(text))
                        allText.AppendLine(text);
                }

                var sections = new Dictionary<string, string> { { "Content", allText.ToString() } };
                _logger.LogInformation("Extracted full content from DOCX");
                return sections;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting from DOCX: {FileName}", file.FileName);
                return new Dictionary<string, string>();
            }
        });
    }

    private async Task<Dictionary<string, string>> ExtractFromPdfAsync(IFormFile file)
    {
        return await Task.Run(() =>
        {
            try
            {
                using var stream = file.OpenReadStream();
                using var pdf = UglyToad.PdfPig.PdfDocument.Open(stream);
                
                var allText = string.Join("\n", pdf.GetPages().Select(p => p.Text));
                var sections = new Dictionary<string, string> { { "Content", allText } };

                _logger.LogInformation("Extracted content from PDF with {PageCount} pages", pdf.NumberOfPages);
                return sections;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting from PDF: {FileName}", file.FileName);
                return new Dictionary<string, string>();
            }
        });
    }
}
