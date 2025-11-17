using CsvHelper;
using ClosedXML.Excel;
using System.Globalization;

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

    private async Task<List<AllContactRow>> ParseCsvAsync(IFormFile file)
    {
        var rows = new List<AllContactRow>();
        using var reader = new StreamReader(file.OpenReadStream());
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        await foreach (var record in csv.GetRecordsAsync<AllContactRow>())
        {
            rows.Add(record);
        }
        return rows;
    }

    private async Task<List<AllContactRow>> ParseExcelAsync(IFormFile file)
    {
        var rows = new List<AllContactRow>();
        using var stream = file.OpenReadStream();
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheet(1); // Assume first sheet

        // Skip header row
        var headerRow = worksheet.Row(1);
        var rowsData = worksheet.RowsUsed().Skip(1);

        foreach (var row in rowsData)
        {
            var contactRow = new AllContactRow
            {
                Category = row.Cell(1).GetValue<string>(),
                Service = row.Cell(2).GetValue<string>(),
                Function = row.Cell(3).GetValue<string>(),
                Embassy = row.Cell(4).GetValue<string>(),
                Name = row.Cell(5).GetValue<string>(),
                Designation = row.Cell(6).GetValue<string>(),
                Info = row.Cell(7).GetValue<string>(),
                Address = row.Cell(8).GetValue<string>(),
                Website = row.Cell(9).GetValue<string>(),
                Phone = row.Cell(10).GetValue<string>(),
                Email = row.Cell(11).GetValue<string>()
            };
            rows.Add(contactRow);
        }
        return rows;
    }

    public async Task UploadAllContactsAsync(IFormFile file)
    {
        _logger.LogInformation("Starting all contacts parsing from file: {FileName}", file.FileName);

        var extension = Path.GetExtension(file.FileName).ToLower();
        List<AllContactRow> rows;

        if (extension == ".csv")
        {
            rows = await ParseCsvAsync(file);
        }
        else if (extension == ".xlsx" || extension == ".xls")
        {
            rows = await ParseExcelAsync(file);
        }
        else
        {
            throw new InvalidOperationException("Unsupported file type");
        }

        _logger.LogInformation("Parsed {Count} rows from file", rows.Count);

        var parsedData = ParseAllContacts(rows);

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

    public async Task DeleteAllContactsAsync()
    {
        _logger.LogInformation("Deleting all contacts");
        await _repository.DeleteAllAsync();
        _logger.LogInformation("All contacts deleted successfully");
    }

    private AllContactsResponse ParseAllContacts(List<AllContactRow> rows)
    {
        var response = new AllContactsResponse();

        foreach (var row in rows)
        {
            switch (row.Category.ToLower())
            {
                case "emergency":
                    response.Emergency.Add(new EmergencyContact
                    {
                        Service = row.Service,
                        Info = row.Info
                    });
                    break;
                case "lafarge":
                    response.Lafarge.Add(new LafargeContact
                    {
                        Function = row.Function,
                        Name = row.Name,
                        Phone = row.Phone
                    });
                    break;
                case "embassies":
                    response.Embassies.Add(new EmbassyContact
                    {
                        Embassy = row.Embassy,
                        Address = row.Address,
                        Website = row.Website,
                        Phone = row.Phone
                    });
                    break;
                case "hr":
                    response.Hr.Add(new HrContact
                    {
                        Name = row.Name,
                        Designation = row.Designation,
                        Email = row.Email
                    });
                    break;
            }
        }

        return response;
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

    private class AllContactRow
    {
        public string Category { get; set; } = string.Empty;
        public string Service { get; set; } = string.Empty;
        public string Function { get; set; } = string.Empty;
        public string Embassy { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string Info { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

}