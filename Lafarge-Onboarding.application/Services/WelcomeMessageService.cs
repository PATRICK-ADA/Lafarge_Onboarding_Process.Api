namespace Lafarge_Onboarding.application.Services;

public sealed class WelcomeMessageService : IWelcomeMessageService
{
    private readonly IWelcomeMessageRepository _repository;
    private readonly IDocumentsUploadService _documentService;
    private readonly ILogger<WelcomeMessageService> _logger;

    public WelcomeMessageService(
        IWelcomeMessageRepository repository,
        IDocumentsUploadService documentService,
        ILogger<WelcomeMessageService> logger)
    {
        _repository = repository;
        _documentService = documentService;
        _logger = logger;
    }

    public async Task<WelcomeMessageResponse> ExtractAndSaveWelcomeMessagesAsync(IFormFileCollection files)
    {
        _logger.LogInformation("Starting welcome messages extraction from {Count} files", files.Count);

        if (files.Count < 2)
        {
            throw new ArgumentException("At least two files are required: one for CEO and one for HR");
        }

        // Assume first file is CEO, second is HR
        var ceoText = await _documentService.ExtractTextFromDocumentAsync(files[0]);
        var hrText = await _documentService.ExtractTextFromDocumentAsync(files[1]);

        _logger.LogInformation("Texts extracted successfully");

        var parsedData = ParseWelcomeMessages(ceoText, hrText);

        var entity = MapToEntity(parsedData);
        await _repository.AddAsync(entity);

        _logger.LogInformation("Welcome messages saved successfully with ID: {Id}", entity.Id);
        return parsedData;
    }

    public async Task<WelcomeMessageResponse?> GetWelcomeMessagesAsync()
    {
        _logger.LogInformation("Retrieving latest welcome messages");

        var entity = await _repository.GetLatestAsync();
        if (entity == null)
        {
            _logger.LogInformation("No welcome messages found");
            return null;
        }

        var response = MapToResponse(entity);
        _logger.LogInformation("Welcome messages retrieved successfully");
        return response;
    }

    public async Task DeleteLatestAsync()
    {
        _logger.LogInformation("Deleting latest welcome messages");
        await _repository.DeleteLatestAsync();
        _logger.LogInformation("Latest welcome messages deleted successfully");
    }

    private WelcomeMessageResponse ParseWelcomeMessages(string ceoText, string hrText)
    {
        var response = new WelcomeMessageResponse();

        // Parse CEO
        response.Ceo.Name = ExtractField(ceoText, "name");
        response.Ceo.Title = ExtractField(ceoText, "title");
        response.Ceo.Message = ExtractField(ceoText, "message");
        response.Ceo.ImageUrl = string.Empty; // Placeholder

        // Parse HR
        response.Hr.Name = ExtractField(hrText, "name");
        response.Hr.Title = ExtractField(hrText, "title");
        response.Hr.Message = ExtractField(hrText, "message");
        response.Hr.ImageUrl = string.Empty; // Placeholder

        return response;
    }

    private string ExtractField(string text, string fieldName)
    {
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var searchTerm = fieldName + ":";

        foreach (var line in lines)
        {
            if (line.ToLower().Contains(searchTerm))
            {
                return line.Substring(line.ToLower().IndexOf(searchTerm) + searchTerm.Length).Trim();
            }
        }

        return string.Empty;
    }

    private WelcomeMessage MapToEntity(WelcomeMessageResponse response)
    {
        return new WelcomeMessage
        {
            CeoName = response.Ceo.Name,
            CeoTitle = response.Ceo.Title,
            CeoImageUrl = response.Ceo.ImageUrl,
            CeoMessage = response.Ceo.Message,
            HrName = response.Hr.Name,
            HrTitle = response.Hr.Title,
            HrImageUrl = response.Hr.ImageUrl,
            HrMessage = response.Hr.Message
        };
    }

    private WelcomeMessageResponse MapToResponse(WelcomeMessage entity)
    {
        return new WelcomeMessageResponse
        {
            Ceo = new WelcomePerson
            {
                Name = entity.CeoName,
                Title = entity.CeoTitle,
                ImageUrl = entity.CeoImageUrl,
                Message = entity.CeoMessage
            },
            Hr = new WelcomePerson
            {
                Name = entity.HrName,
                Title = entity.HrTitle,
                ImageUrl = entity.HrImageUrl,
                Message = entity.HrMessage
            }
        };
    }
}