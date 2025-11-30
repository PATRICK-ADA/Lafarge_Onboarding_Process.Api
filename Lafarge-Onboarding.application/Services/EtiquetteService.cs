namespace Lafarge_Onboarding.application.Services;

public sealed class EtiquetteService : IEtiquetteService
{
    private readonly IEtiquetteRepository _repository;
    private readonly IDocumentsUploadService _documentService;
    private readonly IAuditService _auditService;
    private readonly ILogger<EtiquetteService> _logger;

    public EtiquetteService(
        IEtiquetteRepository repository,
        IDocumentsUploadService documentService,
        IAuditService auditService,
        ILogger<EtiquetteService> logger)
    {
        _repository = repository;
        _documentService = documentService;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<EtiquetteResponse> ExtractAndSaveEtiquetteAsync(IFormFile file)
    {
        _logger.LogInformation("Starting etiquette extraction from file: {FileName}", file.FileName);

        await _repository.DeleteAllAsync();
        _logger.LogInformation("Deleted all existing etiquette records");

        var extractedText = await _documentService.ExtractTextFromDocumentAsync(file);
        _logger.LogInformation("Text extracted successfully. Length: {Length}", extractedText.Length);

        var parsedData = ParseEtiquette(extractedText);
        var entity = MapToEntity(parsedData);
        await _repository.AddAsync(entity);

        await _auditService.LogAuditEventAsync("UPLOAD", "Etiquette", entity.Id.ToString(), "Uploaded etiquette document");

        _logger.LogInformation("Etiquette saved successfully with ID: {Id}", entity.Id);
        return parsedData;
    }

    public async Task<EtiquetteResponse?> GetEtiquetteAsync()
    {
        _logger.LogInformation("Retrieving latest etiquette");

        var response = await _repository.GetLatestAsync();
        if (response == null)
        {
            _logger.LogInformation("No etiquette found");
            return null;
        }

        await _auditService.LogAuditEventAsync("RETRIEVE", "Etiquette", "latest", "Retrieved etiquette");
        _logger.LogInformation("Etiquette retrieved successfully");
        return response;
    }

    public async Task DeleteLatestAsync()
    {
        _logger.LogInformation("Deleting latest etiquette");
        var entity = await _repository.GetLatestAsync();
        if (entity != null)
        {
            await _repository.DeleteLatestAsync();
            await _auditService.LogAuditEventAsync("DELETE", "Etiquette", entity.Id.ToString(), "Deleted latest etiquette");
            _logger.LogInformation("Latest etiquette deleted successfully");
        }
        else
        {
            _logger.LogInformation("No etiquette to delete");
        }
    }

    private EtiquetteResponse ParseEtiquette(string text)
    {
        // Split text into lines for processing
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                       .Select(line => line.Trim())
                       .Where(line => !string.IsNullOrWhiteSpace(line))
                       .ToArray();

        return new EtiquetteResponse
        {
            RegionalInfo = ParseRegionalInfo(lines),
            FirstImpression = ParseFirstImpression(lines)
        };
    }

    private List<RegionalInfoItem> ParseRegionalInfo(string[] lines)
    {
        var regionalInfo = new List<RegionalInfoItem>();
        var startIndex = -1;

        // Find the "UNDERSTANDING THE REGIONS WHERE WE OPERATE" heading
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].ToUpper().Contains("UNDERSTANDING THE REGIONS WHERE WE OPERATE"))
            {
                startIndex = i + 1;
                _logger.LogDebug("Found 'UNDERSTANDING THE REGIONS WHERE WE OPERATE' at line {LineIndex}: '{Line}'", i, lines[i]);
                break;
            }
        }

        if (startIndex == -1)
        {
            _logger.LogWarning("Heading 'UNDERSTANDING THE REGIONS WHERE WE OPERATE' not found in document");
            return regionalInfo;
        }

        var categories = new List<string>();
        var allData = new List<List<string>>();

    
        int currentIndex = startIndex;
        while (currentIndex < lines.Length)
        {
            var line = lines[currentIndex].Trim();

            
            if (line.ToUpper().Contains("MAKING A GOOD FIRST IMPRESSION"))
            {
                break;
            }

        
            if (line.Equals("Category", StringComparison.OrdinalIgnoreCase) ||
                line.Equals("South West", StringComparison.OrdinalIgnoreCase) ||
                line.Equals("South (Mfamosing)", StringComparison.OrdinalIgnoreCase) ||
                line.Equals("North-East (Ashaka)", StringComparison.OrdinalIgnoreCase) ||
                string.IsNullOrWhiteSpace(line))
            {
                currentIndex++;
                continue;
            }

            if (IsCategoryLine(line))
            {
                categories.Add(line);
                var categoryData = new List<string>();

                
                for (int j = 1; j <= 3 && currentIndex + j < lines.Length; j++)
                {
                    var dataLine = lines[currentIndex + j].Trim();
                    if (!string.IsNullOrWhiteSpace(dataLine) && !IsCategoryLine(dataLine))
                    {
                        categoryData.Add(dataLine);
                    }
                    else
                    {
                        categoryData.Add("");
                    }
                }

                allData.Add(categoryData);
                _logger.LogDebug("Found category '{Category}' with {DataCount} data lines", line, categoryData.Count);
                currentIndex += 4; 
            }
            else
            {
                currentIndex++;
            }
        }

    
        for (int i = 0; i < categories.Count && i < allData.Count; i++)
        {
            var data = allData[i];
            var item = new RegionalInfoItem
            {
                Title = categories[i],
                Regions = new List<Lafarge_Onboarding.domain.Dtos.OnboardingResponses.Region>
                {
                    new Lafarge_Onboarding.domain.Dtos.OnboardingResponses.Region { Title = "South West", Content = data.Count > 0 ? data[0] : "" },
                    new Lafarge_Onboarding.domain.Dtos.OnboardingResponses.Region { Title = "South (Mfamosing)", Content = data.Count > 1 ? data[1] : "" },
                    new Lafarge_Onboarding.domain.Dtos.OnboardingResponses.Region { Title = "North-East (Ashaka)", Content = data.Count > 2 ? data[2] : "" }
                }
            };
            regionalInfo.Add(item);
            _logger.LogDebug("Created regional info item: {Title}", item.Title);
        }

        _logger.LogDebug("Parsed {Count} regional info items", regionalInfo.Count);
        return regionalInfo;
    }

    private bool IsCategoryLine(string line)
    {
        
        if (line.Length > 50) return false;
        if (line.Contains("Plant") || line.Contains("depot") || line.Contains("tribe") ||
            line.Contains("language") || line.Contains("Lagos") || line.Contains("Mfamosing") ||
            line.Contains("Ashaka") || line.Contains("Yoruba") || line.Contains("Efik") ||
            line.Contains("Fulani")) return false;
        return true;
    }

    private List<FirstImpressionItem> ParseFirstImpression(string[] lines)
    {
        var firstImpression = new List<FirstImpressionItem>();
        var startIndex = -1;

        
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].ToUpper().Contains("MAKING A GOOD FIRST IMPRESSION"))
            {
                startIndex = i + 1;
                _logger.LogDebug("Found 'MAKING A GOOD FIRST IMPRESSION' at line {LineIndex}: '{Line}'", i, lines[i]);
                break;
            }
        }

        if (startIndex == -1)
        {
            _logger.LogWarning("Heading 'MAKING A GOOD FIRST IMPRESSION' not found in document");
            return firstImpression;
        }

        // Collect all non-empty lines after the heading as bullet points
        for (int i = startIndex; i < lines.Length; i++)
        {
            var line = lines[i].Trim();

            if (string.IsNullOrWhiteSpace(line))
                continue;

            // Check for bullet points (•, -, or just plain text that looks like a bullet)
            if (line.StartsWith("•") || line.StartsWith("-") || (!line.Contains("MAKING A GOOD") && line.Length > 10))
            {
                // Clean the line
                var cleanLine = line.TrimStart('•', '-', ' ').Trim();

                // Split on colon to separate title and content
                var colonIndex = cleanLine.IndexOf(':');
                if (colonIndex > 0)
                {
                    var title = cleanLine.Substring(0, colonIndex).Trim();
                    var content = cleanLine.Substring(colonIndex + 1).Trim();

                    firstImpression.Add(new FirstImpressionItem
                    {
                        Title = title,
                        Content = content
                    });
                    _logger.LogDebug("Added first impression item: {Title}", title);
                }
                else
                {
                    // If no colon, treat the whole line as title
                    firstImpression.Add(new FirstImpressionItem
                    {
                        Title = cleanLine,
                        Content = ""
                    });
                    _logger.LogDebug("Added first impression item (no content): {Title}", cleanLine);
                }
            }
        }

        _logger.LogDebug("Parsed {Count} first impression items", firstImpression.Count);
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