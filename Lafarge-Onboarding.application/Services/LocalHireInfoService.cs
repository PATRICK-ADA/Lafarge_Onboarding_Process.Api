namespace Lafarge_Onboarding.application.Services;

public sealed class LocalHireInfoService : ILocalHireInfoService
{
    private readonly ILocalHireInfoRepository _repository;
    private readonly IImprovedDocumentExtractionService _extractionService;
    private readonly ILogger<LocalHireInfoService> _logger;

    public LocalHireInfoService(
        ILocalHireInfoRepository repository,
        IImprovedDocumentExtractionService extractionService,
        ILogger<LocalHireInfoService> logger)
    {
        _repository = repository;
        _extractionService = extractionService;
        _logger = logger;
    }

    public async Task<LocalHireInfoResponse> ExtractAndSaveLocalHireInfoAsync(IFormFile file)
    {
        _logger.LogInformation("Starting local hire info extraction from file: {FileName}", file.FileName);

        await _repository.DeleteAllAsync();
        _logger.LogInformation("Deleted all existing local hire info records");

        var sections = await _extractionService.ExtractStructuredSectionsAsync(file);
        _logger.LogInformation("Extracted {SectionCount} sections from document", sections.Count);

        var parsedData = ParseLocalHireInfo(sections);
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

    private LocalHireInfoResponse ParseLocalHireInfo(Dictionary<string, string> sections)
    {
        var response = new LocalHireInfoResponse();
        var fullText = string.Join("\n", sections.Values);
        var lines = fullText.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                           .Select(line => line.Trim())
                           .Where(line => !string.IsNullOrWhiteSpace(line))
                           .ToArray();

        _logger.LogInformation("Total lines extracted: {Count}", lines.Length);

        response.AboutLafarge.WhoWeAre = ExtractSection(lines, "1. WHO WE ARE", "2. LAFARGE AFRICA FOOTPRINT");
        response.AboutLafarge.Footprint.Summary = ExtractSection(lines, "2. LAFARGE AFRICA FOOTPRINT", "Cement Plants");
        response.AboutLafarge.Footprint.Plants = ExtractList(lines, "Cement Plants", "ReadyMix Locations");
        response.AboutLafarge.Footprint.ReadyMix = ExtractList(lines, "ReadyMix Locations", "Depots");
        response.AboutLafarge.Footprint.Depots = ExtractSection(lines, "Depots", "3. LAFARGE AFRICA CULTURE");

        response.AboutLafarge.Culture.Summary = ExtractSection(lines, "3. LAFARGE AFRICA CULTURE", "Behavioural Pillars");
        response.AboutLafarge.Culture.Pillars = ExtractSection(lines, "Behavioural Pillars", "Huaxin Spirit");
        response.AboutLafarge.Culture.HuaxinSpirit = ExtractList(lines, "Huaxin Spirit", "• Innovation:");
        response.AboutLafarge.Culture.Innovation = ExtractSection(lines, "• Innovation:", "Respectful Workplaces");
        response.AboutLafarge.Culture.RespectfulWorkplaces = ExtractSection(lines, "Respectful Workplaces", "1.1. General introduction");

        response.GeneralIntro.Introduction = ExtractSection(lines, "1.1. General introduction", "Country:");
        response.GeneralIntro.CountryFacts = ExtractCountryFacts(lines);
        response.GeneralIntro.InterestingFacts = ExtractList(lines, "Interesting Facts About Nigeria", "National Holidays");
        response.GeneralIntro.Holidays = ExtractHolidays(lines);

        return response;
    }

    private string ExtractSection(string[] lines, string startHeading, string stopHeading)
    {
        var startIndex = FindHeadingIndex(lines, startHeading);
        if (startIndex == -1)
        {
            _logger.LogWarning("Heading '{Heading}' not found", startHeading);
            return string.Empty;
        }

        var stopIndex = FindHeadingIndex(lines, stopHeading);
        var endIndex = stopIndex == -1 ? lines.Length : stopIndex;

        var content = new List<string>();
        for (int i = startIndex + 1; i < endIndex; i++)
        {
            content.Add(lines[i]);
        }

        var result = string.Join(" ", content).Trim();
        _logger.LogInformation("Extracted section '{StartHeading}' to '{StopHeading}': {Count} chars", startHeading, stopHeading, result.Length);
        return result;
    }

    private List<string> ExtractList(string[] lines, string startHeading, string stopHeading)
    {
        var startIndex = FindHeadingIndex(lines, startHeading);
        if (startIndex == -1)
        {
            _logger.LogWarning("List heading '{Heading}' not found", startHeading);
            return new List<string>();
        }

        var stopIndex = FindHeadingIndex(lines, stopHeading);
        var endIndex = stopIndex == -1 ? lines.Length : stopIndex;

        _logger.LogInformation("Extracting list from '{StartHeading}' (line {Start}) to '{StopHeading}' (line {End})", 
            startHeading, startIndex, stopHeading, stopIndex);

        var list = new List<string>();
        for (int i = startIndex + 1; i < endIndex; i++)
        {
            var line = lines[i];
            if (line.Trim().StartsWith("•") || line.Trim().StartsWith("◦") || line.Trim().StartsWith("-"))
            {
                var trimmed = line.Trim().TrimStart('•', '◦', '-', ' ');

                while (i + 1 < endIndex)
                {
                    var nextLine = lines[i + 1].Trim();
                    if (string.IsNullOrWhiteSpace(nextLine) || nextLine.StartsWith("•") || nextLine.StartsWith("◦") || nextLine.StartsWith("-"))
                        break;
                    trimmed += " " + nextLine;
                    i++;
                }

                list.Add(trimmed);
            }
        }

        _logger.LogInformation("Extracted {Count} list items for '{Heading}'", list.Count, startHeading);
        return list;
    }

    private List<CountryFact> ExtractCountryFacts(string[] lines)
    {
        var facts = new List<CountryFact>();
        var startIndex = FindHeadingIndex(lines, "Country:");
        if (startIndex == -1)
        {
            _logger.LogWarning("Heading 'Country:' not found");
            return facts;
        }

        var stopIndex = FindHeadingIndex(lines, "Interesting Facts About Nigeria");
        var endIndex = stopIndex == -1 ? lines.Length : stopIndex;

        string currentLabel = null!;
        for (int i = startIndex + 1; i < endIndex; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (line.EndsWith(":"))
            {
                currentLabel = line.TrimEnd(':').Trim();
            }
            else if (currentLabel != null)
            {
                facts.Add(new CountryFact { Label = currentLabel, Value = line });
                currentLabel = null!;
            }
        }

        return facts;
    }

    private List<Holiday> ExtractHolidays(string[] lines)
    {
        var holidays = new List<Holiday>();
        var startIndex = FindHeadingIndex(lines, "National Holidays");
        if (startIndex == -1)
        {
            _logger.LogWarning("Heading 'National Holidays' not found");
            return holidays;
        }

        for (int i = startIndex + 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line) || line.Contains("Visiting Nigeria"))
                continue;

            var match = Regex.Match(line, @"^(\d{1,2}(?:-\d{1,2})?)\s+(January|February|March|April|May|June|July|August|September|October|November|December)(.*)$");
            if (match.Success)
            {
                var date = match.Groups[1].Value + " " + match.Groups[2].Value;
                var name = match.Groups[3].Value.Trim();
                holidays.Add(new Holiday { Date = date, Name = name });
            }
        }

        return holidays;
    }

    private int FindHeadingIndex(string[] lines, string heading)
    {
        var headingUpper = heading.ToUpper().Replace(" ", "");
        for (int i = 0; i < lines.Length; i++)
        {
            var lineUpper = lines[i].ToUpper().Replace(" ", "");
            if (lineUpper == headingUpper || lineUpper.StartsWith(headingUpper))
            {
                _logger.LogInformation("Found heading '{Heading}' at line {Index}: '{Line}'", heading, i, lines[i]);
                return i;
            }
        }
        _logger.LogWarning("Heading '{Heading}' not found in document", heading);
        return -1;
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
            Pillars = response.AboutLafarge.Culture.Pillars,
            Innovation = response.AboutLafarge.Culture.Innovation,
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
                    Plants = JsonSerializer.Deserialize<List<string>>(entity.Plants ?? string.Empty) ?? new List<string>(),
                    ReadyMix = JsonSerializer.Deserialize<List<string>>(entity.ReadyMix ?? string.Empty) ?? new List<string>(),
                    Depots = entity.Depots
                },
                Culture = new Culture
                {
                    Summary = entity.CultureSummary,
                    Pillars = entity.Pillars ?? string.Empty,
                    Innovation = entity.Innovation,
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
