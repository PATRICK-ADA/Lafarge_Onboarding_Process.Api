namespace Lafarge_Onboarding.application.Services;

public sealed class OnboardingPlanService : IOnboardingPlanService
{
    private readonly IOnboardingPlanRepository _repository;
    private readonly IDocumentsUploadService _documentService;
    private readonly IAuditService _auditService;
    private readonly ILogger<OnboardingPlanService> _logger;

    public OnboardingPlanService(
        IOnboardingPlanRepository repository,
        IDocumentsUploadService documentService,
        IAuditService auditService,
        ILogger<OnboardingPlanService> logger)
    {
        _repository = repository;
        _documentService = documentService;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<OnboardingPlanResponse> ExtractAndSaveOnboardingPlanAsync(IFormFile file)
    {
        _logger.LogInformation("Starting onboarding plan extraction from file: {FileName}", file.FileName);

        await _repository.DeleteAllAsync();
        _logger.LogInformation("Deleted all existing onboarding plan records");

        var extractedText = await _documentService.ExtractTextFromDocumentAsync(file);
        _logger.LogInformation("Text extracted successfully. Length: {Length}", extractedText.Length);

        var parsedData = ParseOnboardingPlan(extractedText);
        var entity = MapToEntity(parsedData);
        await _repository.AddAsync(entity);

        await _auditService.LogAuditEventAsync("UPLOAD", "OnboardingPlan", entity.Id.ToString(), "Uploaded onboarding plan document");

        _logger.LogInformation("Onboarding plan saved successfully with ID: {Id}", entity.Id);
        return parsedData;
    }

    public async Task<OnboardingPlanResponse?> GetOnboardingPlanAsync()
    {
        _logger.LogInformation("Retrieving latest onboarding plan");

        var response = await _repository.GetLatestAsync();
        if (response == null)
        {
            _logger.LogInformation("No onboarding plan found");
            return null;
        }

        await _auditService.LogAuditEventAsync("RETRIEVE", "OnboardingPlan", "latest", "Retrieved onboarding plan");
        _logger.LogInformation("Onboarding plan retrieved successfully");
        return response;
    }

    public async Task DeleteLatestAsync()
    {
        _logger.LogInformation("Deleting latest onboarding plan");
        var entity = await _repository.GetLatestAsync();
        if (entity != null)
        {
            await _repository.DeleteLatestAsync();
            await _auditService.LogAuditEventAsync("DELETE", "OnboardingPlan", entity.Id.ToString(), "Deleted latest onboarding plan");
            _logger.LogInformation("Latest onboarding plan deleted successfully");
        }
        else
        {
            _logger.LogInformation("No onboarding plan to delete");
        }
    }

    private OnboardingPlanResponse ParseOnboardingPlan(string text)
    {
       
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                       .Select(line => line.Trim())
                       .Where(line => !string.IsNullOrWhiteSpace(line))
                       .ToArray();

        return new OnboardingPlanResponse
        {
            Buddy = new Buddy
            {
                Details = ExtractBuddyDetails(lines),
                Activities = ExtractBuddyActivities(lines)
            },
            Checklist = new Checklist
            {
                Summary = ExtractChecklistSummary(lines),
                Timeline = ExtractTimeline(lines)
            }
        };
    }

    private string ExtractBuddyDetails(string[] lines)
    {
        var startIndex = -1;
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].ToUpper().Contains("YOUR ONBOARDING BUDDY"))
            {
                startIndex = i + 1;
                break;
            }
        }

        if (startIndex == -1) return string.Empty;

        var content = new List<string>();
        for (int i = startIndex; i < lines.Length; i++)
        {
            var line = lines[i];
     
            if (line.Trim().StartsWith("•") || line.ToUpper().Contains("NEW HIRE CHECKLIST"))
            {
                break;
            }
            content.Add(line);
        }

        return string.Join(" ", content).Trim();
    }

    private List<string> ExtractBuddyActivities(string[] lines)
    {
        var activities = new List<string>();
        var startIndex = -1;

       
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].ToUpper().Contains("YOUR ONBOARDING BUDDY"))
            {
                startIndex = i + 1;
                _logger.LogDebug("Found 'YOUR ONBOARDING BUDDY' at line {LineIndex}: '{Line}'", i, lines[i]);
                break;
            }
        }

        if (startIndex == -1)
        {
            _logger.LogWarning("Heading 'YOUR ONBOARDING BUDDY' not found in document");
            return activities;
        }

        
        bool foundIntro = false;
        for (int i = startIndex; i < lines.Length; i++)
        {
            var line = lines[i];
      
            if (line.ToUpper().Contains("NEW HIRE CHECKLIST"))
            {
                _logger.LogDebug("Stopping buddy activities extraction at line {LineIndex} due to: '{Line}'", i, line);
                break;
            }

        
            if (string.IsNullOrWhiteSpace(line))
                continue;

           
            if (!foundIntro && line.Contains("You may also want to consult"))
            {
                foundIntro = true;
                _logger.LogDebug("Found introduction at line {LineIndex}: '{Line}'", i, line);
                continue;
            }

            
            if (foundIntro)
            {
                var activity = line.Trim();
                if (!string.IsNullOrWhiteSpace(activity))
                {
                    activities.Add(activity);
                    _logger.LogDebug("Found buddy activity: '{Activity}'", activity);
                }
            }
        }

        _logger.LogDebug("Extracted {Count} buddy activities", activities.Count);
        return activities;
    }

    private string ExtractChecklistSummary(string[] lines)
    {
        var startIndex = -1;
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].ToUpper().Contains("NEW HIRE CHECKLIST"))
            {
                startIndex = i + 1;
                break;
            }
        }

        if (startIndex == -1) return string.Empty;

        var content = new List<string>();
        for (int i = startIndex; i < lines.Length; i++)
        {
            var line = lines[i];
           
            if (line.Contains("Day One:") || line.Contains("First week") ||
                line.Contains("Within first month") || line.Contains("Within first three"))
            {
                break;
            }
            content.Add(line);
        }

        return string.Join(" ", content).Trim();
    }


    private List<TimelineItem> ExtractTimeline(string[] lines)
    {
        var timeline = new List<TimelineItem>();
        var startIndex = -1;

        
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].ToUpper().Contains("NEW HIRE CHECKLIST"))
            {
                startIndex = i + 1;
                break;
            }
        }

        if (startIndex == -1) return timeline;

        TimelineItem? currentItem = null;

        for (int i = startIndex; i < lines.Length; i++)
        {
            var line = lines[i];

      
            if (line.Contains("Day One:") || line.Contains("First week") ||
                line.Contains("Within first month") || line.Contains("Within first three"))
            {
                if (currentItem != null)
                {
                    timeline.Add(currentItem);
                    _logger.LogDebug("Added timeline item: {Period} with {TaskCount} tasks and {SubTaskCount} sub-tasks",
                        currentItem.Period, currentItem.Tasks.Count, currentItem.SubTasks.Count);
                }

                var period = line.Trim();
                if (period.Contains(":"))
                {
                    period = period.Split(':')[0].Trim() + ":";
                }

                currentItem = new TimelineItem
                {
                    Period = period,
                    Tasks = new List<string>(),
                    SubTasks = new List<string>()
                };
                _logger.LogDebug("Started new timeline period: '{Period}'", period);
            }
            else if (currentItem != null && !string.IsNullOrWhiteSpace(line.Trim()) &&
                      !line.Contains("Day One:") && !line.Contains("First week") &&
                      !line.Contains("Within first month") && !line.Contains("Within first three"))
            {
                var trimmed = line.Trim();

              
                if (trimmed.StartsWith("○") || trimmed.StartsWith("  ") || trimmed.StartsWith("\t"))
                {
                    trimmed = trimmed.TrimStart('○', ' ', '\t');
                    currentItem.SubTasks.Add(trimmed);
                    _logger.LogDebug("Found sub-task: '{SubTask}'", trimmed);
                }
                else
                {
                    currentItem.Tasks.Add(trimmed);
                    _logger.LogDebug("Found task: '{Task}'", trimmed);
                }
            }
            else if (currentItem != null && !string.IsNullOrWhiteSpace(line.Trim()) &&
                      !line.Trim().StartsWith("•") && !line.Trim().StartsWith("-") &&
                      !line.Contains("Day One:") && !line.Contains("First week") &&
                      !line.Contains("Within first month") && !line.Contains("Within first three"))
            {
               
                if (currentItem.Tasks.Count > 0)
                {
                    currentItem.Tasks[currentItem.Tasks.Count - 1] += " " + line.Trim();
                    _logger.LogDebug("Continued task: '{Task}'", currentItem.Tasks[currentItem.Tasks.Count - 1]);
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