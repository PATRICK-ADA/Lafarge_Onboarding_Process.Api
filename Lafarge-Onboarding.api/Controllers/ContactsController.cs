namespace Lafarge_Onboarding.api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ContactsController : ControllerBase
{
    private readonly IContactService _contactService;
    private readonly IAllContactService _allContactService;

    public ContactsController(IContactService contactService, IAllContactService allContactService)
    {
        _contactService = contactService;
        _allContactService = allContactService;
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

    [HttpGet("get-local")]
    [Authorize(Roles = "HR_ADMIN")]
    public async Task<IActionResult> GetLocalContacts()
    {
        var contacts = await _contactService.GetLocalContactsAsync();
        return Ok(ApiResponse<List<ContactDto>>.Success(contacts));
    }

    [HttpPost("upload-all")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> UploadAllContacts(IFormFile file)
    {
        var allowedExtensions = new[] { ".pdf", ".docx", ".txt" };
        var fileExtension = Path.GetExtension(file?.FileName ?? "").ToLower();
        
        return (file == null || file.Length == 0)
            ? BadRequest(ApiResponse<object>.Failure("File is required"))
            : !allowedExtensions.Contains(fileExtension)
                ? BadRequest(ApiResponse<object>.Failure("Invalid file type. Only PDF, DOCX, and TXT files are allowed."))
                : Ok(ApiResponse<string>.Success(await Task.Run(async () => { await _allContactService.UploadAllContactsAsync(file); return "All contacts uploaded successfully"; })));
    }

    [HttpGet("get-all")]
    [Authorize(Roles = "HR_ADMIN")]
    public async Task<IActionResult> GetAllContacts()
    {
        var contacts = await _allContactService.GetAllContactsAsync();
        return Ok(ApiResponse<AllContactsResponse>.Success(contacts));
    }

    [HttpDelete("delete-local")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> DeleteLocalContacts()
    {
        await _contactService.DeleteAllContactsAsync();
        return Ok(ApiResponse<object>.Success("Local contacts deleted successfully"));
    }

    [HttpDelete("delete-all")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> DeleteAllContacts()
    {
        await _allContactService.DeleteAllContactsAsync();
        return Ok(ApiResponse<object>.Success("All contacts deleted successfully"));
    }
}