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
        var extractedText = await _documentService.ExtractTextFromDocumentAsync(file.FileName);
        _logger.LogInformation("Text extracted successfully. Length: {Length}", extractedText.Length);

        
        var parsedData = ParseLocalHireInfo(extractedText);

        
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

    private LocalHireInfoResponse ParseLocalHireInfo(string text)
    {
        
        var response = new LocalHireInfoResponse();

        // Parse AboutLafarge section
        response.AboutLafarge.WhoWeAre = ExtractSection(text, "whoWeAre");
        response.AboutLafarge.Footprint.Summary = ExtractSection(text, "footprint", "summary");
        response.AboutLafarge.Footprint.Plants = ExtractList(text, "plants");
        response.AboutLafarge.Footprint.ReadyMix = ExtractList(text, "readyMix");
        response.AboutLafarge.Footprint.Depots = ExtractSection(text, "depots");

        response.AboutLafarge.Culture.Summary = ExtractSection(text, "culture", "summary");
        response.AboutLafarge.Culture.Pillars = ExtractList(text, "pillars");
        response.AboutLafarge.Culture.Innovation = ExtractList(text, "innovation");
        response.AboutLafarge.Culture.HuaxinSpirit = ExtractList(text, "huaxinSpirit");
        response.AboutLafarge.Culture.RespectfulWorkplaces = ExtractSection(text, "respectfulWorkplaces");

        // Parse GeneralIntro section
        response.GeneralIntro.Introduction = ExtractSection(text, "introduction");
        response.GeneralIntro.CountryFacts = ExtractCountryFacts(text);
        response.GeneralIntro.InterestingFacts = ExtractList(text, "interestingFacts");
        response.GeneralIntro.Holidays = ExtractHolidays(text);

        return response;
    }

    private string ExtractSection(string text, params string[] keywords)
    {
        // Simple keyword-based extraction
        
        var searchText = string.Join(" ", keywords).ToLower();
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            if (line.ToLower().Contains(searchText))
            {
                return line.Trim();
            }
        }

        return string.Empty;
    }

    private List<string> ExtractList(string text, string keyword)
    {
        // Simple list extraction - assumes lists are marked with bullets or numbers
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var list = new List<string>();
        var inList = false;

        foreach (var line in lines)
        {
            if (line.ToLower().Contains(keyword.ToLower()))
            {
                inList = true;
                continue;
            }

            if (inList && (line.Trim().StartsWith("-") || line.Trim().StartsWith("•") ||
                          (line.Trim().Length > 0 && char.IsDigit(line.Trim()[0]))))
            {
                var trimmed = line.Trim().TrimStart('-', '•', ' ');
                if (trimmed.Length > 0 && char.IsDigit(trimmed[0]))
                {
                    trimmed = trimmed[1..].TrimStart();
                }
                list.Add(trimmed);
            }
            else if (inList && !string.IsNullOrWhiteSpace(line) &&
                    !line.Trim().StartsWith("-") && !line.Trim().StartsWith("•") &&
                    !char.IsDigit(line.Trim()[0]))
            {
                // End of list
                break;
            }
        }

        return list;
    }

    private List<CountryFact> ExtractCountryFacts(string text)
    {
        var facts = new List<CountryFact>();
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
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

    private List<Holiday> ExtractHolidays(string text)
    {
        var holidays = new List<Holiday>();
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            // Look for date patterns like "Jan 1 - New Year's Day"
            var parts = line.Split('-', 2);
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