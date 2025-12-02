

namespace Lafarge_Onboarding.application.Services;

public sealed class ContactService : IContactService
{
    private readonly IContactRepository _repository;
    private readonly ILogger<ContactService> _logger;
    private readonly IAuditService _auditService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ContactService(
        IContactRepository repository,
        ILogger<ContactService> logger,
        IAuditService auditService,
        IHttpContextAccessor httpContextAccessor)
    {
        _repository = repository;
        _logger = logger;
        _auditService = auditService;
        _httpContextAccessor = httpContextAccessor;
    }
    private string GetStatus()
    {
        return _httpContextAccessor.HttpContext.Response.StatusCode >= 200 && _httpContextAccessor.HttpContext.Response.StatusCode < 300 ? "Success" : "Failed";
    }


    public async Task UploadContactsAsync(IFormFile file)
    {
        _logger.LogInformation("Uploading contacts from file: {FileName}", file.FileName);

        var contacts = await ParseContactsFromFileAsync(file);

        var entities = contacts.Select(c => new Contact
        {
            Name = c.Name,
            Email = c.Email,
            Phone = c.Phone
        }).ToList();

        await _repository.AddRangeAsync(entities);

        _logger.LogInformation("Contacts uploaded successfully");

        var status = GetStatus();

        await _auditService.LogAuditEventAsync(
            action: "CREATE",
            resourceType: "Contact",
            resourceId: _httpContextAccessor.HttpContext?.Request?.Path.ToString(),
            status: status,
            newValues: JsonSerializer.Serialize(entities));
    }

    public async Task<List<ContactDto>> GetLocalContactsAsync()
    {
        _logger.LogInformation("Retrieving local contacts");

        var dtos = await _repository.GetAllAsync();

        _logger.LogInformation("Retrieved {Count} contacts", dtos.Count);

        var status = GetStatus();

        await _auditService.LogAuditEventAsync(
            action: "READ",
            resourceType: "Contact",
            resourceId: _httpContextAccessor.HttpContext?.Request?.Path.ToString(),
            status: status);

        return dtos;
    }

    public async Task DeleteAllContactsAsync()
    {
        _logger.LogInformation("Deleting all local contacts");

        var contacts = await _repository.GetAllAsync();
        var oldValues = JsonSerializer.Serialize(contacts);

        await _repository.DeleteAllAsync();

        _logger.LogInformation("All local contacts deleted successfully");

        var status = GetStatus();

        await _auditService.LogAuditEventAsync(
            action: "DELETE",
            resourceType: "Contact",
            resourceId: _httpContextAccessor.HttpContext?.Request?.Path.ToString(),
            oldValues: oldValues,
            newValues: null,
            status: status
        );
    }

    private async Task<List<ContactDto>> ParseContactsFromFileAsync(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).ToLower();
        if (extension == ".csv")
        {
            return await ParseCsvAsync(file);
        }
        else if (extension == ".xlsx" || extension == ".xls")
        {
            return await ParseExcelAsync(file);
        }
        else
        {
            throw new InvalidOperationException("Unsupported file type. Only CSV and Excel files are supported.");
        }
    }

    private Task<List<ContactDto>> ParseCsvAsync(IFormFile file)
    {
        var contacts = new List<ContactDto>();
        using var reader = new StreamReader(file.OpenReadStream());
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csv.GetRecords<ContactDto>();
        contacts.AddRange(records);
        return Task.FromResult(contacts);
    }

    private Task<List<ContactDto>> ParseExcelAsync(IFormFile file)
    {
        var contacts = new List<ContactDto>();
        using var stream = file.OpenReadStream();
        using var document = SpreadsheetDocument.Open(stream, false);
        var worksheet = document.WorkbookPart?.WorksheetParts.First().Worksheet;
        var rows = worksheet!.Descendants<Row>();

        bool isFirstRow = true;
        foreach (var row in rows)
        {
            if (isFirstRow)
            {
                isFirstRow = false;
                continue; // Skip header
            }

            var cells = row.Descendants<Cell>().ToList();
            if (cells.Count >= 3)
            {
                var contact = new ContactDto
                {
                    Name = GetCellValue(cells[0], document.WorkbookPart!),
                    Email = GetCellValue(cells[1], document.WorkbookPart!),
                    Phone = GetCellValue(cells[2], document.WorkbookPart!)
                };
                contacts.Add(contact);
            }
        }
        return Task.FromResult(contacts);
    }

    private string GetCellValue(Cell cell, WorkbookPart? workbookPart)
    {
        if (cell.CellValue == null) return string.Empty;
        var value = cell.CellValue.Text;
        if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString && workbookPart != null)
        {
            return workbookPart.SharedStringTablePart?.SharedStringTable.Elements<SharedStringItem>().ElementAt(int.Parse(value)).Text?.Text ?? string.Empty;
        }
        return value;
    }
}