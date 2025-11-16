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
        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse<object>.Failure("File is required"));
        }

        var allowedExtensions = new[] { ".xlsx", ".xls", ".csv" };
        var fileExtension = Path.GetExtension(file.FileName).ToLower();
        if (!allowedExtensions.Contains(fileExtension))
        {
            return BadRequest(ApiResponse<object>.Failure("Invalid file type. Only Excel (.xlsx, .xls) and CSV files are allowed."));
        }

        await _contactService.UploadContactsAsync(file);
        return Ok(ApiResponse<string>.Success("Contacts uploaded successfully"));
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
        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse<object>.Failure("File is required"));
        }

        var allowedExtensions = new[] { ".pdf", ".docx", ".txt" }; // Assuming document types
        var fileExtension = Path.GetExtension(file.FileName).ToLower();
        if (!allowedExtensions.Contains(fileExtension))
        {
            return BadRequest(ApiResponse<object>.Failure("Invalid file type. Only PDF, DOCX, and TXT files are allowed."));
        }

        await _allContactService.UploadAllContactsAsync(file);
        return Ok(ApiResponse<string>.Success("All contacts uploaded successfully"));
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
        return Ok(ApiResponse<object>.Success(null, "Local contacts deleted successfully"));
    }

    [HttpDelete("delete-all")]
    [Authorize(Roles = "HR_ADMIN")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> DeleteAllContacts()
    {
        await _allContactService.DeleteAllContactsAsync();
        return Ok(ApiResponse<object>.Success(null, "All contacts deleted successfully"));
    }
}