namespace Lafarge_Onboarding.application.Services;

public sealed class LocalHireInfoService : ILocalHireInfoService
{
    private readonly ILocalHireInfoRepository _repository;
    private readonly IImprovedDocumentExtractionService _extractionService;
    private readonly IAuditService _auditService;
    private readonly ILogger<LocalHireInfoService> _logger;

    public LocalHireInfoService(
        ILocalHireInfoRepository repository,
        IImprovedDocumentExtractionService extractionService,
        IAuditService auditService,
        ILogger<LocalHireInfoService> logger)
    {
        _repository = repository;
        _extractionService = extractionService;
        _auditService = auditService;
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

        await _auditService.LogAuditEventAsync("UPLOAD", "LocalHireInfo", entity.Id.ToString(), "Uploaded local hire info document");

        _logger.LogInformation("Local hire info saved successfully with ID: {Id}", entity.Id);
        return parsedData;
    }

    public async Task<LocalHireInfoResponse?> GetLocalHireInfoAsync()
    {
        _logger.LogInformation("Retrieving latest local hire info");

        var response = await _repository.GetLatestAsync();
        if (response == null)
        {
            _logger.LogInformation("No local hire info found");
            return null;
        }

        await _auditService.LogAuditEventAsync("RETRIEVE", "LocalHireInfo", "latest", "Retrieved local hire info");
        _logger.LogInformation("Local hire info retrieved successfully");
        return response;
    }

    public async Task DeleteLatestAsync()
    {
        _logger.LogInformation("Deleting latest local hire info");
        var entity = await _repository.GetLatestAsync();
        if (entity != null)
        {
            await _repository.DeleteLatestAsync();
            await _auditService.LogAuditEventAsync("DELETE", "LocalHireInfo", entity.Id.ToString(), "Deleted latest local hire info");
            _logger.LogInformation("Latest local hire info deleted successfully");
        }
        else
        {
            _logger.LogInformation("No local hire info to delete");
        }
    }

    private LocalHireInfoResponse ParseLocalHireInfo(Dictionary<string, string> sections)
    {
        var fullText = string.Join("\n", sections.Values);
        var lines = fullText.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                           .Select(line => line.Trim())
                           .Where(line => !string.IsNullOrWhiteSpace(line))
                           .ToArray();

        _logger.LogInformation("Total lines extracted: {Count}", lines.Length);

        return new LocalHireInfoResponse
        {
            AboutLafarge = new AboutLafarge
            {
                WhoWeAre = ExtractSection(lines, "1. WHO WE ARE", "2. LAFARGE AFRICA FOOTPRINT"),
                Footprint = new Footprint
                {
                    Summary = ExtractSection(lines, "2. LAFARGE AFRICA FOOTPRINT", "Cement Plants"),
                    Plants = ExtractList(lines, "Cement Plants", "ReadyMix Locations"),
                    ReadyMix = ExtractList(lines, "ReadyMix Locations", "Depots"),
                    Depots = ExtractSection(lines, "Depots", "3. LAFARGE AFRICA CULTURE")
                },
                Culture = new Culture
                {
                    Summary = ExtractSection(lines, "3. LAFARGE AFRICA CULTURE", "Behavioural Pillars"),
                    Pillars = ExtractSection(lines, "Behavioural Pillars", "Huaxin Spirit"),
                    HuaxinSpirit = ExtractList(lines, "Huaxin Spirit", "• Innovation:"),
                    Innovation = ExtractSection(lines, "• Innovation:", "Respectful Workplaces"),
                    RespectfulWorkplaces = ExtractSection(lines, "Respectful Workplaces", "1.1. General introduction")
                }
            },
            GeneralIntro = new GeneralIntro
            {
                Introduction = ExtractSection(lines, "1.1. General introduction", "Country:"),
                CountryFacts = ExtractCountryFacts(lines),
                InterestingFacts = ExtractList(lines, "Interesting Facts About Nigeria", "National Holidays"),
                Holidays = ExtractHolidays(lines)
            }
        };
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
