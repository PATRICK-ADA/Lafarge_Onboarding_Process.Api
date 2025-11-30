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
            throw new ArgumentException("At least two files are required: one for CEO and one for HR");

        await _repository.DeleteAllAsync();
        _logger.LogInformation("Deleted all existing welcome messages records");

        var ceoText = await _documentService.ExtractTextFromDocumentAsync(files[0]);
        var hrText = await _documentService.ExtractTextFromDocumentAsync(files[1]);

        var parsedData = ParseWelcomeMessages(ceoText, hrText);
        var entity = MapToEntity(parsedData);
        await _repository.AddAsync(entity);

        _logger.LogInformation("Welcome messages saved successfully with ID: {Id}", entity.Id);
        return parsedData;
    }

    public async Task<WelcomeMessageResponse?> GetWelcomeMessagesAsync()
    {
        _logger.LogInformation("Retrieving latest welcome messages");

        var response = await _repository.GetLatestAsync();
        if (response == null)
        {
            _logger.LogInformation("No welcome messages found");
            return null;
        }

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
        // Parse CEO
        var ceoParsed = ParsePersonText(ceoText);
        
        // Parse HR
        var hrParsed = ParsePersonText(hrText);

        return new WelcomeMessageResponse
        {
            Ceo = new WelcomePerson
            {
                Name = ceoParsed.Name,
                Title = ceoParsed.Title,
                Message = ceoParsed.Message,
                ImageUrl = string.Empty
            },
            Hr = new WelcomePerson
            {
                Name = hrParsed.Name,
                Title = hrParsed.Title,
                Message = hrParsed.Message,
                ImageUrl = string.Empty
            }
        };
    }


    private (string Name, string Title, string Message) ParsePersonText(string text)
    {
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (lines.Length < 2)
        {
            return (string.Empty, string.Empty, text);
        }

        var title = lines[^1]; // Last line
        var nameLine = lines[^2]; // Second to last
        var name = nameLine.TrimStart('•', ' ').Trim(); // Remove bullet and spaces

        var messageLines = lines.Take(lines.Length - 2);
        var message = string.Join(" ", messageLines);

        return (name, title, message);
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