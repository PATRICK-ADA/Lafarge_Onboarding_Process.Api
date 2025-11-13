namespace Lafarge_Onboarding.application.Services;

public sealed class AllContactService : IAllContactService
{
    private readonly IAllContactRepository _repository;
    private readonly IDocumentsUploadService _documentService;
    private readonly ILogger<AllContactService> _logger;

    public AllContactService(
        IAllContactRepository repository,
        IDocumentsUploadService documentService,
        ILogger<AllContactService> logger)
    {
        _repository = repository;
        _documentService = documentService;
        _logger = logger;
    }

    public async Task UploadAllContactsAsync(IFormFile file)
    {
        _logger.LogInformation("Starting all contacts extraction from file: {FileName}", file.FileName);

        var extractedText = await _documentService.ExtractTextFromDocumentAsync(file.FileName);
        _logger.LogInformation("Text extracted successfully. Length: {Length}", extractedText.Length);

        var parsedData = ParseAllContacts(extractedText);

        await _repository.DeleteAllAsync(); // Clear existing

        var entities = MapToEntities(parsedData);
        await _repository.AddRangeAsync(entities);

        _logger.LogInformation("All contacts saved successfully");
    }

    public async Task<AllContactsResponse> GetAllContactsAsync()
    {
        _logger.LogInformation("Retrieving all contacts");

        var entities = await _repository.GetAllAsync();
        var response = MapToResponse(entities);

        _logger.LogInformation("All contacts retrieved successfully");
        return response;
    }

    private AllContactsResponse ParseAllContacts(string text)
    {
        var response = new AllContactsResponse();

        // Parse emergency contacts
        response.Emergency = ExtractEmergencyContacts(text);

        // Parse lafarge contacts
        response.Lafarge = ExtractLafargeContacts(text);

        // Parse embassies
        response.Embassies = ExtractEmbassyContacts(text);

        // Parse hr contacts
        response.Hr = ExtractHrContacts(text);

        return response;
    }

    private List<EmergencyContact> ExtractEmergencyContacts(string text)
    {
        // Simple extraction - assume sections are marked
        var contacts = new List<EmergencyContact>();
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        bool inEmergency = false;
        foreach (var line in lines)
        {
            if (line.ToLower().Contains("emergency") || line.ToLower().Contains("fire services"))
            {
                inEmergency = true;
                continue;
            }
            if (inEmergency && line.Contains(":"))
            {
                var parts = line.Split(':', 2);
                if (parts.Length == 2)
                {
                    contacts.Add(new EmergencyContact
                    {
                        Service = parts[0].Trim(),
                        Info = parts[1].Trim()
                    });
                }
            }
            else if (inEmergency && string.IsNullOrWhiteSpace(line))
            {
                break; // End of section
            }
        }
        return contacts;
    }

    private List<LafargeContact> ExtractLafargeContacts(string text)
    {
        var contacts = new List<LafargeContact>();
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        bool inLafarge = false;
        foreach (var line in lines)
        {
            if (line.ToLower().Contains("lafarge") || line.ToLower().Contains("country ceo"))
            {
                inLafarge = true;
                continue;
            }
            if (inLafarge && line.Contains("-"))
            {
                var parts = line.Split('-', 2);
                if (parts.Length == 2)
                {
                    contacts.Add(new LafargeContact
                    {
                        Function = parts[0].Trim(),
                        Name = parts[1].Trim(),
                        Phone = "" // Assume phone is in name or separate
                    });
                }
            }
            else if (inLafarge && string.IsNullOrWhiteSpace(line))
            {
                break;
            }
        }
        return contacts;
    }

    private List<EmbassyContact> ExtractEmbassyContacts(string text)
    {
        var contacts = new List<EmbassyContact>();
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        bool inEmbassies = false;
        foreach (var line in lines)
        {
            if (line.ToLower().Contains("embass"))
            {
                inEmbassies = true;
                continue;
            }
            if (inEmbassies && line.Contains(":"))
            {
                var parts = line.Split(':', 2);
                if (parts.Length == 2)
                {
                    contacts.Add(new EmbassyContact
                    {
                        Embassy = parts[0].Trim(),
                        Address = parts[1].Trim(),
                        Website = "",
                        Phone = ""
                    });
                }
            }
            else if (inEmbassies && string.IsNullOrWhiteSpace(line))
            {
                break;
            }
        }
        return contacts;
    }

    private List<HrContact> ExtractHrContacts(string text)
    {
        var contacts = new List<HrContact>();
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        bool inHr = false;
        foreach (var line in lines)
        {
            if (line.ToLower().Contains("hr") || line.ToLower().Contains("human resources"))
            {
                inHr = true;
                continue;
            }
            if (inHr && line.Contains(","))
            {
                var parts = line.Split(',', 2);
                if (parts.Length == 2)
                {
                    contacts.Add(new HrContact
                    {
                        Name = parts[0].Trim(),
                        Designation = parts[1].Trim(),
                        Email = ""
                    });
                }
            }
            else if (inHr && string.IsNullOrWhiteSpace(line))
            {
                break;
            }
        }
        return contacts;
    }

    private List<AllContact> MapToEntities(AllContactsResponse response)
    {
        var entities = new List<AllContact>();

        foreach (var contact in response.Emergency)
        {
            entities.Add(new AllContact
            {
                Category = "emergency",
                Data = JsonSerializer.Serialize(contact)
            });
        }

        foreach (var contact in response.Lafarge)
        {
            entities.Add(new AllContact
            {
                Category = "lafarge",
                Data = JsonSerializer.Serialize(contact)
            });
        }

        foreach (var contact in response.Embassies)
        {
            entities.Add(new AllContact
            {
                Category = "embassies",
                Data = JsonSerializer.Serialize(contact)
            });
        }

        foreach (var contact in response.Hr)
        {
            entities.Add(new AllContact
            {
                Category = "hr",
                Data = JsonSerializer.Serialize(contact)
            });
        }

        return entities;
    }

    private AllContactsResponse MapToResponse(List<AllContact> entities)
    {
        var response = new AllContactsResponse();

        foreach (var entity in entities)
        {
            switch (entity.Category)
            {
                case "emergency":
                    response.Emergency.Add(JsonSerializer.Deserialize<EmergencyContact>(entity.Data) ?? new EmergencyContact());
                    break;
                case "lafarge":
                    response.Lafarge.Add(JsonSerializer.Deserialize<LafargeContact>(entity.Data) ?? new LafargeContact());
                    break;
                case "embassies":
                    response.Embassies.Add(JsonSerializer.Deserialize<EmbassyContact>(entity.Data) ?? new EmbassyContact());
                    break;
                case "hr":
                    response.Hr.Add(JsonSerializer.Deserialize<HrContact>(entity.Data) ?? new HrContact());
                    break;
            }
        }

        return response;
    }
}