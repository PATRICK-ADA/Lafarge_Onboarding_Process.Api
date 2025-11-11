namespace Lafarge_Onboarding.application.Services;

public sealed class OnboardingPlanService : IOnboardingPlanService
{
    private readonly IOnboardingPlanRepository _repository;
    private readonly IDocumentsUploadService _documentService;
    private readonly ILogger<OnboardingPlanService> _logger;

    public OnboardingPlanService(
        IOnboardingPlanRepository repository,
        IDocumentsUploadService documentService,
        ILogger<OnboardingPlanService> logger)
    {
        _repository = repository;
        _documentService = documentService;
        _logger = logger;
    }

    public async Task<OnboardingPlanResponse> ExtractAndSaveOnboardingPlanAsync(IFormFile file)
    {
        _logger.LogInformation("Starting onboarding plan extraction from file: {FileName}", file.FileName);

        // Extract text from the uploaded file
        var extractedText = await _documentService.ExtractTextFromDocumentAsync(file.FileName);
        _logger.LogInformation("Text extracted successfully. Length: {Length}", extractedText.Length);

        var parsedData = ParseOnboardingPlan(extractedText);

        var entity = MapToEntity(parsedData);
        await _repository.AddAsync(entity);

        _logger.LogInformation("Onboarding plan saved successfully with ID: {Id}", entity.Id);
        return parsedData;
    }

    public async Task<OnboardingPlanResponse?> GetOnboardingPlanAsync()
    {
        _logger.LogInformation("Retrieving latest onboarding plan");

        var entity = await _repository.GetLatestAsync();
        if (entity == null)
        {
            _logger.LogInformation("No onboarding plan found");
            return null;
        }

        var response = MapToResponse(entity);
        _logger.LogInformation("Onboarding plan retrieved successfully");
        return response;
    }

    private OnboardingPlanResponse ParseOnboardingPlan(string text)
    {
        var response = new OnboardingPlanResponse();

        // Parse Buddy section
        response.Buddy.Details = ExtractSection(text, "buddy", "details");
        response.Buddy.Activities = ExtractList(text, "buddy", "activities");

        // Parse Checklist section
        response.Checklist.Summary = ExtractSection(text, "checklist", "summary");
        response.Checklist.Timeline = ExtractTimeline(text);

        return response;
    }

    private string ExtractSection(string text, params string[] keywords)
    {
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

    private List<string> ExtractList(string text, params string[] keywords)
    {
        var searchText = string.Join(" ", keywords).ToLower();
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var list = new List<string>();
        var inList = false;

        foreach (var line in lines)
        {
            if (line.ToLower().Contains(searchText))
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

    private List<TimelineItem> ExtractTimeline(string text)
    {
        var timeline = new List<TimelineItem>();
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var inTimeline = false;
        TimelineItem? currentItem = null;

        foreach (var line in lines)
        {
            if (line.ToLower().Contains("timeline"))
            {
                inTimeline = true;
                continue;
            }

            if (inTimeline)
            {
                // Look for period headers like "Day One:", "First Week:"
                if (line.Contains(":") && !line.Trim().StartsWith("-") && !line.Trim().StartsWith("•"))
                {
                    if (currentItem != null)
                    {
                        timeline.Add(currentItem);
                    }
                    currentItem = new TimelineItem
                    {
                        Period = line.Split(':')[0].Trim(),
                        Tasks = new List<string>(),
                        SubTasks = new List<string>()
                    };
                }
                else if (currentItem != null && (line.Trim().StartsWith("-") || line.Trim().StartsWith("•")))
                {
                    var trimmed = line.Trim().TrimStart('-', '•', ' ');
                    if (trimmed.ToLower().Contains("subtask") || trimmed.ToLower().Contains("sub-task"))
                    {
                        // This is a subtask
                        currentItem.SubTasks.Add(trimmed);
                    }
                    else
                    {
                        // This is a task
                        currentItem.Tasks.Add(trimmed);
                    }
                }
            }
        }

        if (currentItem != null)
        {
            timeline.Add(currentItem);
        }

        return timeline;
    }

    private OnboardingPlan MapToEntity(OnboardingPlanResponse response)
    {
        return new OnboardingPlan
        {
            BuddyDetails = response.Buddy.Details,
            BuddyActivities = JsonSerializer.Serialize(response.Buddy.Activities),
            ChecklistSummary = response.Checklist.Summary,
            Timeline = JsonSerializer.Serialize(response.Checklist.Timeline)
        };
    }

    private OnboardingPlanResponse MapToResponse(OnboardingPlan entity)
    {
        return new OnboardingPlanResponse
        {
            Buddy = new Buddy
            {
                Details = entity.BuddyDetails,
                Activities = JsonSerializer.Deserialize<List<string>>(entity.BuddyActivities) ?? new List<string>()
            },
            Checklist = new Checklist
            {
                Summary = entity.ChecklistSummary,
                Timeline = JsonSerializer.Deserialize<List<TimelineItem>>(entity.Timeline) ?? new List<TimelineItem>()
            }
        };
    }
}