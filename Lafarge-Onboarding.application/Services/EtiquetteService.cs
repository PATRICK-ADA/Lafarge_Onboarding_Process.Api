namespace Lafarge_Onboarding.application.Services;

public sealed class EtiquetteService : IEtiquetteService
{
    private readonly IEtiquetteRepository _repository;
    private readonly IDocumentsUploadService _documentService;
    private readonly ILogger<EtiquetteService> _logger;

    public EtiquetteService(
        IEtiquetteRepository repository,
        IDocumentsUploadService documentService,
        ILogger<EtiquetteService> logger)
    {
        _repository = repository;
        _documentService = documentService;
        _logger = logger;
    }

    public async Task<EtiquetteResponse> ExtractAndSaveEtiquetteAsync(IFormFile file)
    {
        _logger.LogInformation("Starting etiquette extraction from file: {FileName}", file.FileName);

        // Extract text from the uploaded file
        var extractedText = await _documentService.ExtractTextFromDocumentAsync(file.FileName);
        _logger.LogInformation("Text extracted successfully. Length: {Length}", extractedText.Length);

        var parsedData = ParseEtiquette(extractedText);

        var entity = MapToEntity(parsedData);
        await _repository.AddAsync(entity);

        _logger.LogInformation("Etiquette saved successfully with ID: {Id}", entity.Id);
        return parsedData;
    }

    public async Task<EtiquetteResponse?> GetEtiquetteAsync()
    {
        _logger.LogInformation("Retrieving latest etiquette");

        var entity = await _repository.GetLatestAsync();
        if (entity == null)
        {
            _logger.LogInformation("No etiquette found");
            return null;
        }

        var response = MapToResponse(entity);
        _logger.LogInformation("Etiquette retrieved successfully");
        return response;
    }

    private EtiquetteResponse ParseEtiquette(string text)
    {
        var response = new EtiquetteResponse();

        // Parse Regional Info
        response.RegionalInfo = ParseRegionalInfo(text);

        // Parse First Impression
        response.FirstImpression = ParseFirstImpression(text);

        return response;
    }

    private List<RegionalInfoItem> ParseRegionalInfo(string text)
    {
        var regionalInfo = new List<RegionalInfoItem>();
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        RegionalInfoItem? currentItem = null;
        Region? currentRegion = null;

        foreach (var line in lines)
        {
            if (line.ToLower().Contains("lafarge presence"))
            {
                currentItem = new RegionalInfoItem { Title = "Lafarge Presence", Regions = new List<Region>() };
                regionalInfo.Add(currentItem);
            }
            else if (currentItem != null && line.Contains(":") && !line.Trim().StartsWith("-"))
            {
                // New region
                var parts = line.Split(':', 2);
                if (parts.Length == 2)
                {
                    currentRegion = new Region
                    {
                        Title = parts[0].Trim(),
                        Content = parts[1].Trim()
                    };
                    currentItem.Regions.Add(currentRegion);
                }
            }
            else if (currentRegion != null && !string.IsNullOrWhiteSpace(line.Trim()))
            {
                // Continue content
                currentRegion.Content += " " + line.Trim();
            }
        }

        return regionalInfo;
    }

    private List<FirstImpressionItem> ParseFirstImpression(string text)
    {
        var firstImpression = new List<FirstImpressionItem>();
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        FirstImpressionItem? currentItem = null;

        foreach (var line in lines)
        {
            if (line.ToLower().Contains("greetings") || line.ToLower().Contains("body language"))
            {
                if (currentItem != null)
                {
                    firstImpression.Add(currentItem);
                }
                currentItem = new FirstImpressionItem
                {
                    Title = line.Split(':')[0].Trim(),
                    Content = line.Contains(":") ? line.Split(':', 2)[1].Trim() : ""
                };
            }
            else if (currentItem != null && !string.IsNullOrWhiteSpace(line.Trim()))
            {
                // Continue content
                currentItem.Content += " " + line.Trim();
            }
        }

        if (currentItem != null)
        {
            firstImpression.Add(currentItem);
        }

        return firstImpression;
    }

    private Etiquette MapToEntity(EtiquetteResponse response)
    {
        return new Etiquette
        {
            RegionalInfo = JsonSerializer.Serialize(response.RegionalInfo),
            FirstImpression = JsonSerializer.Serialize(response.FirstImpression)
        };
    }

    private EtiquetteResponse MapToResponse(Etiquette entity)
    {
        return new EtiquetteResponse
        {
            RegionalInfo = JsonSerializer.Deserialize<List<RegionalInfoItem>>(entity.RegionalInfo) ?? new List<RegionalInfoItem>(),
            FirstImpression = JsonSerializer.Deserialize<List<FirstImpressionItem>>(entity.FirstImpression) ?? new List<FirstImpressionItem>()
        };
    }
}