namespace Lafarge_Onboarding.application.Services;

public sealed class LocalHireInfoService : ILocalHireInfoService
{
    private readonly ILocalHireInfoRepository _repository;
    private readonly IDocumentsUploadService _documentService;
    private readonly ILogger<LocalHireInfoService> _logger;

    public LocalHireInfoService(
        ILocalHireInfoRepository repository,
        IDocumentsUploadService documentService,
        ILogger<LocalHireInfoService> logger)
    {
        _repository = repository;
        _documentService = documentService;
        _logger = logger;
    }

    public async Task<LocalHireInfoResponse> ExtractAndSaveLocalHireInfoAsync(IFormFile file)
    {
        _logger.LogInformation("Starting local hire info extraction from file: {FileName}", file.FileName);

        // Extract text from the uploaded file
        var extractedText = await _documentService.ExtractTextFromDocumentAsync(file);
        _logger.LogInformation("Text extracted successfully. Length: {Length}", extractedText.Length);
        _logger.LogDebug("Extracted text preview: {Preview}", extractedText.Length > 500 ? extractedText.Substring(0, 500) + "..." : extractedText);

        var parsedData = ParseLocalHireInfo(extractedText);
        _logger.LogInformation("Parsed data - WhoWeAre: '{WhoWeAre}', FootprintSummary: '{FootprintSummary}'", parsedData.AboutLafarge.WhoWeAre, parsedData.AboutLafarge.Footprint.Summary);

        var entity = MapToEntity(parsedData);
        await _repository.AddAsync(entity);

        _logger.LogInformation("Local hire info saved successfully with ID: {Id}", entity.Id);
        return parsedData;
    }

    public async Task<LocalHireInfoResponse?> GetLocalHireInfoAsync()
    {
        _logger.LogInformation("Retrieving latest local hire info");

        var entity = await _repository.GetLatestAsync();
        if (entity == null)
        {
            _logger.LogInformation("No local hire info found");
            return null;
        }

        var response = MapToResponse(entity);
        _logger.LogInformation("Local hire info retrieved successfully");
        return response;
    }

    public async Task DeleteLatestAsync()
    {
        _logger.LogInformation("Deleting latest local hire info");
        await _repository.DeleteLatestAsync();
        _logger.LogInformation("Latest local hire info deleted successfully");
    }

    private LocalHireInfoResponse ParseLocalHireInfo(string text)
    {
        var response = new LocalHireInfoResponse();

        // Split text into lines for processing
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                       .Select(line => line.Trim())
                       .Where(line => !string.IsNullOrWhiteSpace(line))
                       .ToArray();

        // Extract sections based on numbered headings
        response.AboutLafarge.WhoWeAre = ExtractSectionByHeading(lines, "1. WHO WE ARE");
        response.AboutLafarge.Footprint.Summary = ExtractSectionByHeading(lines, "2. LAFARGE AFRICA FOOTPRINT");
        response.AboutLafarge.Footprint.Plants = ExtractListByHeading(lines, "Cement Plants");
        response.AboutLafarge.Footprint.ReadyMix = ExtractListByHeading(lines, "ReadyMix Locations");
        response.AboutLafarge.Footprint.Depots = ExtractSectionByHeading(lines, "Depots");

        response.AboutLafarge.Culture.Summary = ExtractSectionByHeading(lines, "3. LAFARGE AFRICA CULTURE");
        response.AboutLafarge.Culture.Pillars = ExtractListByHeading(lines, "Behavioural Pillars");
        response.AboutLafarge.Culture.HuaxinSpirit = ExtractListByHeading(lines, "Huaxin Spirit");
        response.AboutLafarge.Culture.Innovation = ExtractListByHeading(lines, "Innovation");
        response.AboutLafarge.Culture.RespectfulWorkplaces = ExtractSectionByHeading(lines, "Respectful Workplaces");

        response.GeneralIntro.Introduction = ExtractSectionByHeading(lines, "1.1. General introduction");
        response.GeneralIntro.CountryFacts = ExtractCountryFacts(lines);
        response.GeneralIntro.InterestingFacts = ExtractListByHeading(lines, "Interesting Facts About Nigeria");
        response.GeneralIntro.Holidays = ExtractHolidays(lines);

        return response;
    }

    private string ExtractSectionByHeading(string[] lines, string heading)
    {
        var startIndex = -1;
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].ToUpper().Contains(heading.ToUpper()))
            {
                startIndex = i + 1;
                _logger.LogDebug("Found heading '{Heading}' at line {LineIndex}: '{Line}'", heading, i, lines[i]);
                break;
            }
        }

        if (startIndex == -1)
        {
            _logger.LogWarning("Heading '{Heading}' not found in document", heading);
            return string.Empty;
        }

        var content = new List<string>();
        for (int i = startIndex; i < lines.Length; i++)
        {
            var line = lines[i];
            // Stop at next numbered section or major heading
            if (Regex.IsMatch(line, @"^\d+\.\s") || line.ToUpper().Contains("GENERAL INTRODUCTION") ||
                line.ToUpper().Contains("COUNTRY:") || line.ToUpper().Contains("INTERESTING FACTS") ||
                line.ToUpper().Contains("NATIONAL HOLIDAYS"))
            {
                _logger.LogDebug("Stopping extraction for '{Heading}' at line {LineIndex} due to: '{Line}'", heading, i, line);
                break;
            }
            content.Add(line);
        }

        var result = string.Join(" ", content).Trim();
        _logger.LogDebug("Extracted content for '{Heading}': '{Content}'", heading, result.Length > 100 ? result.Substring(0, 100) + "..." : result);
        return result;
    }

    private List<string> ExtractListByHeading(string[] lines, string heading)
    {
        var startIndex = -1;
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].ToUpper().Contains(heading.ToUpper()))
            {
                startIndex = i + 1;
                break;
            }
        }

        if (startIndex == -1) return new List<string>();

        var list = new List<string>();
        for (int i = startIndex; i < lines.Length; i++)
        {
            var line = lines[i];
            // Stop at next section
            if (Regex.IsMatch(line, @"^\d+\.\s") || line.ToUpper().Contains("GENERAL INTRODUCTION") ||
                line.ToUpper().Contains("COUNTRY:") || line.ToUpper().Contains("INTERESTING FACTS") ||
                line.ToUpper().Contains("NATIONAL HOLIDAYS"))
            {
                break;
            }

            // Check for bullet points or numbered items
            if (line.Trim().StartsWith("•") || line.Trim().StartsWith("-") ||
                (line.Trim().Length > 0 && char.IsDigit(line.Trim()[0])))
            {
                var trimmed = line.Trim().TrimStart('•', '-', ' ');
                if (trimmed.Length > 0 && char.IsDigit(trimmed[0]))
                {
                    // Remove number and dot if present
                    var dotIndex = trimmed.IndexOf('.');
                    if (dotIndex > 0)
                    {
                        trimmed = trimmed[(dotIndex + 1)..].Trim();
                    }
                }
                list.Add(trimmed);
            }
        }

        return list;
    }

    private List<CountryFact> ExtractCountryFacts(string[] lines)
    {
        var facts = new List<CountryFact>();
        var startIndex = -1;

        // Find the "Country:" heading
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].ToUpper().Contains("COUNTRY:"))
            {
                startIndex = i + 1;
                break;
            }
        }

        if (startIndex == -1) return facts;

        for (int i = startIndex; i < lines.Length; i++)
        {
            var line = lines[i];
            // Stop at next major section
            if (line.ToUpper().Contains("INTERESTING FACTS") || line.ToUpper().Contains("NATIONAL HOLIDAYS"))
            {
                break;
            }

            if (line.Contains(":"))
            {
                var parts = line.Split(':', 2);
                if (parts.Length == 2)
                {
                    facts.Add(new CountryFact
                    {
                        Label = parts[0].Trim(),
                        Value = parts[1].Trim()
                    });
                }
            }
        }

        return facts;
    }

    private List<Holiday> ExtractHolidays(string[] lines)
    {
        var holidays = new List<Holiday>();
        var startIndex = -1;

        // Find the "National Holidays" heading
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].ToUpper().Contains("NATIONAL HOLIDAYS"))
            {
                startIndex = i + 1;
                break;
            }
        }

        if (startIndex == -1) return holidays;

        for (int i = startIndex; i < lines.Length; i++)
        {
            var line = lines[i];
            // Look for date patterns like "01 January	New Year's Day"
            var parts = line.Split('\t', 2);
            if (parts.Length == 2)
            {
                holidays.Add(new Holiday
                {
                    Date = parts[0].Trim(),
                    Name = parts[1].Trim()
                });
            }
        }

        return holidays;
    }

    private LocalHireInfo MapToEntity(LocalHireInfoResponse response)
    {
        return new LocalHireInfo
        {
            WhoWeAre = response.AboutLafarge.WhoWeAre,
            FootprintSummary = response.AboutLafarge.Footprint.Summary,
            Plants = JsonSerializer.Serialize(response.AboutLafarge.Footprint.Plants),
            ReadyMix = JsonSerializer.Serialize(response.AboutLafarge.Footprint.ReadyMix),
            Depots = response.AboutLafarge.Footprint.Depots,
            CultureSummary = response.AboutLafarge.Culture.Summary,
            Pillars = JsonSerializer.Serialize(response.AboutLafarge.Culture.Pillars),
            Innovation = JsonSerializer.Serialize(response.AboutLafarge.Culture.Innovation),
            HuaxinSpirit = JsonSerializer.Serialize(response.AboutLafarge.Culture.HuaxinSpirit),
            RespectfulWorkplaces = response.AboutLafarge.Culture.RespectfulWorkplaces,
            Introduction = response.GeneralIntro.Introduction,
            CountryFacts = JsonSerializer.Serialize(response.GeneralIntro.CountryFacts),
            InterestingFacts = JsonSerializer.Serialize(response.GeneralIntro.InterestingFacts),
            Holidays = JsonSerializer.Serialize(response.GeneralIntro.Holidays)
        };
    }

    private LocalHireInfoResponse MapToResponse(LocalHireInfo entity)
    {
        return new LocalHireInfoResponse
        {
            AboutLafarge = new AboutLafarge
            {
                WhoWeAre = entity.WhoWeAre,
                Footprint = new Footprint
                {
                    Summary = entity.FootprintSummary,
                    Plants = JsonSerializer.Deserialize<List<string>>(entity.Plants) ?? new List<string>(),
                    ReadyMix = JsonSerializer.Deserialize<List<string>>(entity.ReadyMix) ?? new List<string>(),
                    Depots = entity.Depots
                },
                Culture = new Culture
                {
                    Summary = entity.CultureSummary,
                    Pillars = JsonSerializer.Deserialize<List<string>>(entity.Pillars) ?? new List<string>(),
                    Innovation = JsonSerializer.Deserialize<List<string>>(entity.Innovation) ?? new List<string>(),
                    HuaxinSpirit = JsonSerializer.Deserialize<List<string>>(entity.HuaxinSpirit) ?? new List<string>(),
                    RespectfulWorkplaces = entity.RespectfulWorkplaces
                }
            },
            GeneralIntro = new GeneralIntro
            {
                Introduction = entity.Introduction,
                CountryFacts = JsonSerializer.Deserialize<List<CountryFact>>(entity.CountryFacts) ?? new List<CountryFact>(),
                InterestingFacts = JsonSerializer.Deserialize<List<string>>(entity.InterestingFacts) ?? new List<string>(),
                Holidays = JsonSerializer.Deserialize<List<Holiday>>(entity.Holidays) ?? new List<Holiday>()
            }
        };
    }
}