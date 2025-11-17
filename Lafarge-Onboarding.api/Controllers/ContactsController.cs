namespace Lafarge_Onboarding.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ContactsController : ControllerBase
{
    private readonly IContactService _contactService;
    private readonly IAllContactService _allContactService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ContactsController(IContactService contactService, IAllContactService allContactService, IWebHostEnvironment webHostEnvironment)
    {
        _contactService = contactService;
        _allContactService = allContactService;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpPost("upload-local")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> UploadLocalContacts(IFormFile file)
    {
        var allowedExtensions = new[] { ".xlsx", ".xls", ".csv" };
        var fileExtension = Path.GetExtension(file?.FileName ?? "").ToLower();
        
        return (file == null || file.Length == 0)
            ? BadRequest(ApiResponse<object>.Failure("File is required"))
            : !allowedExtensions.Contains(fileExtension)
                ? BadRequest(ApiResponse<object>.Failure("Invalid file type. Only Excel (.xlsx, .xls) and CSV files are allowed."))
                : Ok(ApiResponse<string>.Success(await Task.Run(async () => { await _contactService.UploadContactsAsync(file); return "Contacts uploaded successfully"; })));
    }


    [HttpPost("upload-all")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> UploadAllContacts(IFormFile file)
    {
        var allowedExtensions = new[] { ".csv", ".xlsx", ".xls" };
        var fileExtension = Path.GetExtension(file?.FileName ?? "").ToLower();

        return (file == null || file.Length == 0)
            ? BadRequest(ApiResponse<object>.Failure("File is required"))
            : !allowedExtensions.Contains(fileExtension)
                ? BadRequest(ApiResponse<object>.Failure("Invalid file type. Only CSV, XLSX, and XLS files are allowed."))
                : Ok(ApiResponse<string>.Success(await Task.Run(async () => { await _allContactService.UploadAllContactsAsync(file); return "All contacts uploaded successfully"; })));
    }


    [HttpGet("get-local")]
    [Authorize(Roles = "HR_ADMIN")]
    public async Task<IActionResult> GetLocalContacts()
    {
        var contacts = await _contactService.GetLocalContactsAsync();
        return Ok(ApiResponse<List<ContactDto>>.Success(contacts));
    }

  

    [HttpGet("get-all")]
    [Authorize(Roles = "HR_ADMIN")]
    public async Task<IActionResult> GetAllContacts()
    {
        var contacts = await _allContactService.GetAllContactsAsync();
        return Ok(ApiResponse<AllContactsResponse>.Success(contacts));
    }

   
    [HttpGet("download-all-contacts-file-format")]
    [Authorize(Roles = "HR_ADMIN")]
    public IActionResult DownloadAllContactsFileFormat()
    {
        var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Formats", "AllContacts-Format.csv");
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound(ApiResponse<object>.Failure("File not found"));
        }
        var fileBytes = System.IO.File.ReadAllBytes(filePath);
        return File(fileBytes, "text/csv", "AllContacts-Format.csv");
    }

    [HttpGet("download-local-contacts-file-format")]
    [Authorize(Roles = "HR_ADMIN")]
    public IActionResult DownloadLocalContactsFileFormat()
    {
        var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Formats", "LocalContacts-Format.csv");
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound(ApiResponse<object>.Failure("File not found"));
        }
        var fileBytes = System.IO.File.ReadAllBytes(filePath);
        return File(fileBytes, "text/csv", "LocalContacts-Format.csv");
    }

    [HttpDelete("delete-all")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> DeleteAllContacts()
    {
        await _allContactService.DeleteAllContactsAsync();
        return Ok(ApiResponse<object>.Success("All contacts deleted successfully"));
    }

    [HttpDelete("delete-local")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> DeleteLocalContacts()
    {
        await _contactService.DeleteAllContactsAsync();
        return Ok(ApiResponse<object>.Success("Local contacts deleted successfully"));
    }

}