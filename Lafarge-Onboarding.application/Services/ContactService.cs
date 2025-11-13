using DocumentFormat.OpenXml.Spreadsheet;

namespace Lafarge_Onboarding.application.Services;

public sealed class ContactService : IContactService
{
    private readonly IContactRepository _repository;
    private readonly ILogger<ContactService> _logger;

    public ContactService(
        IContactRepository repository,
        ILogger<ContactService> logger)
    {
        _repository = repository;
        _logger = logger;
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
    }

    public async Task<List<ContactDto>> GetLocalContactsAsync()
    {
        _logger.LogInformation("Retrieving local contacts");

        var entities = await _repository.GetAllAsync();
        var dtos = entities.Select(c => new ContactDto
        {
            Name = c.Name,
            Email = c.Email,
            Phone = c.Phone
        }).ToList();

        _logger.LogInformation("Retrieved {Count} contacts", dtos.Count);
        return dtos;
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

    private async Task<List<ContactDto>> ParseCsvAsync(IFormFile file)
    {
        var contacts = new List<ContactDto>();
        using var reader = new StreamReader(file.OpenReadStream());
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csv.GetRecords<ContactDto>();
        contacts.AddRange(records);
        return contacts;
    }

    private async Task<List<ContactDto>> ParseExcelAsync(IFormFile file)
    {
        var contacts = new List<ContactDto>();
        using var stream = file.OpenReadStream();
        using var document = SpreadsheetDocument.Open(stream, false);
        var worksheet = document.WorkbookPart.WorksheetParts.First().Worksheet;
        var rows = worksheet.Descendants<Row>();

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
                    Name = GetCellValue(cells[0], document.WorkbookPart),
                    Email = GetCellValue(cells[1], document.WorkbookPart),
                    Phone = GetCellValue(cells[2], document.WorkbookPart)
                };
                contacts.Add(contact);
            }
        }
        return contacts;
    }

    private string GetCellValue(Cell cell, WorkbookPart workbookPart)
    {
        if (cell.CellValue == null) return string.Empty;
        var value = cell.CellValue.Text;
        if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
        {
            return workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(int.Parse(value)).Text?.Text ?? string.Empty;
        }
        return value;
    }
}