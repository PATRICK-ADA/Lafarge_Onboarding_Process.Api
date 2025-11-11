namespace Lafarge_Onboarding.application.Services;

public sealed class UsersService : IUsersService
{
    private readonly IUsersRepository _usersRepository;
    private readonly UserManager<Users> _userManager;
    private readonly RoleManager<Role> _roleManager;

    public UsersService(IUsersRepository usersRepository, UserManager<Users> userManager, RoleManager<Role> roleManager)
    {
        _usersRepository = usersRepository;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<PaginatedResponse<GetUserResponse>> GetUsersAsync(PaginationRequest pagination)
    {
        var (users, totalCount) = await _usersRepository.GetUsersAsync(pagination);
        var userResponses = users.Select(u => new GetUserResponse
        {
            Id = u.Id,
            Name = $"{u.FirstName} {u.LastName}",
            Email = u.Email!,
            PhoneNumber = u.PhoneNumber,
            Role = u.Role,
            Department = null,
            CreatedAt = u.CreatedAt
        });

        return new PaginatedResponse<GetUserResponse>
        {
            Content = userResponses,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<string> UploadBulkUsersAsync(IFormFile file)
    {
        var errors = new List<string>();
        var successCount = 0;

        var fileExtension = Path.GetExtension(file.FileName).ToLower();
        List<CreateUserRequest> userRequests;

        try
        {
            if (fileExtension == ".csv")
            {
                userRequests = await ParseCsvFileAsync(file);
            }
            else if (fileExtension == ".xlsx" || fileExtension == ".xls")
            {
                userRequests = await ParseExcelFileAsync(file);
            }
            else
            {
                throw new InvalidOperationException("Unsupported file type");
            }
        }
        catch (Exception ex)
        {
            return $"Error parsing file: {ex.Message}";
        }

        foreach (var userRequest in userRequests)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(userRequest.Email);
                if (existingUser != null)
                {
                    errors.Add($"User with email {userRequest.Email} already exists");
                    continue;
                }

                // Create new user
                var user = new Users
                {
                    UserName = userRequest.Email,
                    Email = userRequest.Email,
                    FirstName = userRequest.FirstName,
                    LastName = userRequest.LastName,
                    PhoneNumber = userRequest.PhoneNumber,
                    StaffProfilePicture = userRequest.StaffProfilePicture,
                    Role = userRequest.Role,
                    ActiveStatus = userRequest.ActiveStatus,
                    EmailConfirmed = true
                };

                var tempPassword = Guid.NewGuid().ToString("N").Substring(0, 8) + "Temp!";

                var result = await _userManager.CreateAsync(user, tempPassword);
                if (!result.Succeeded)
                {
                    errors.Add($"Failed to create user {userRequest.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    continue;
                }

                var roleName = userRequest.Role;
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    var role = new Role { Name = roleName, Description = $"{roleName} role" };
                    await _roleManager.CreateAsync(role);
                }

                var roleResult = await _userManager.AddToRoleAsync(user, roleName);
                if (!roleResult.Succeeded)
                {
                    errors.Add($"Failed to assign role to user {userRequest.Email}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    continue;
                }

                successCount++;
            }
            catch (Exception ex)
            {
                errors.Add($"Error processing user {userRequest.Email}: {ex.Message}");
            }
        }

        var message = $"Bulk upload completed. {successCount} users created successfully.";
        if (errors.Any())
        {
            message += $" Errors: {string.Join("; ", errors)}";
        }

        return message;
    }

    private async Task<List<CreateUserRequest>> ParseCsvFileAsync(IFormFile file)
    {
        var userRequests = new List<CreateUserRequest>();

        using var reader = new StreamReader(file.OpenReadStream());
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        await csv.ReadAsync();
        csv.ReadHeader();

        while (await csv.ReadAsync())
        {
            var userRequest = new CreateUserRequest
            {
                FirstName = csv.GetField<string>("First Name"),
                LastName = csv.GetField<string>("Last Name"),
                Email = csv.GetField<string>("Email"),
                PhoneNumber = csv.GetField<string>("Phone Number"),
                ActiveStatus = bool.TryParse(csv.GetField<string>("Active Status"), out var activeStatus) ? activeStatus : true,
                StaffProfilePicture = csv.GetField<string>("Staff Profile Picture (Base64)"),
                Role = csv.GetField<string>("Role")
            };

            userRequests.Add(userRequest);
        }

        return userRequests;
    }

    private async Task<List<CreateUserRequest>> ParseExcelFileAsync(IFormFile file)
    {
        var userRequests = new List<CreateUserRequest>();

        using var stream = file.OpenReadStream();
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheet(1); // Assuming data is in the first worksheet

        var rows = worksheet.RowsUsed().Skip(1); // Skip header row

        foreach (var row in rows)
        {
            var userRequest = new CreateUserRequest
            {
                FirstName = row.Cell(1).GetValue<string>(), // First Name
                LastName = row.Cell(2).GetValue<string>(), // Last Name
                Email = row.Cell(3).GetValue<string>(), // Email
                PhoneNumber = row.Cell(4).GetValue<string>(), // Phone Number
                ActiveStatus = bool.TryParse(row.Cell(5).GetValue<string>(), out var activeStatus) ? activeStatus : true, // Active Status
                StaffProfilePicture = row.Cell(6).GetValue<string>(), // Staff Profile Picture (Base64)
                Role = row.Cell(7).GetValue<string>() // Role
            };

            userRequests.Add(userRequest);
        }

        return userRequests;
    }

    public async Task<PaginatedResponse<GetUserResponse>> GetUsersByRoleAsync(string role, PaginationRequest pagination)
    {
        var (users, totalCount) = await _usersRepository.GetUsersByRoleAsync(role, pagination);
        var userResponses = users.Select(u => new GetUserResponse
        {
            Id = u.Id,
            Name = $"{u.FirstName} {u.LastName}",
            Email = u.Email!,
            PhoneNumber = u.PhoneNumber,
            Role = u.Role,
            CreatedAt = u.CreatedAt
        });

        return new PaginatedResponse<GetUserResponse>
        {
            Content = userResponses,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PaginatedResponse<GetUserResponse>> GetUsersByNameAsync(string name, PaginationRequest pagination)
    {
        var (users, totalCount) = await _usersRepository.GetUsersByNameAsync(name, pagination);
        var userResponses = users.Select(u => new GetUserResponse
        {
            Id = u.Id,
            Name = $"{u.FirstName} {u.LastName}",
            Email = u.Email!,
            PhoneNumber = u.PhoneNumber,
            Role = u.Role,
            Department = null,
            CreatedAt = u.CreatedAt
        });

        return new PaginatedResponse<GetUserResponse>
        {
            Content = userResponses,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<GetUserResponse> GetUserByIdAsync(string id)
    {
        var user = await _usersRepository.GetUserByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        var userResponse = new GetUserResponse
        {
            Id = user.Id,
            Name = $"{user.FirstName} {user.LastName}",
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
            Department = null,
            CreatedAt = user.CreatedAt
        };

        return userResponse;
    }

    public async Task<string> UpdateUserByIdAsync(string id, UpdateUserRequest request)
    {
        var result = await _usersRepository.UpdateUserAsync(id, request);
        if (!result)
        {
            throw new KeyNotFoundException("User not found");
        }
        return "User updated successfully";
    }

    public async Task<string> UpdateBulkUsersAsync(UpdateBulkUsersRequest request)
    {
        var errors = new List<string>();
        var successCount = 0;

        foreach (var userItem in request.Users)
        {
            try
            {
                // Validate user exists
                var existingUser = await _usersRepository.GetUserByIdAsync(userItem.Id);
                if (existingUser == null)
                {
                    errors.Add($"User with ID {userItem.Id} does not exist");
                    continue;
                }

                // Update user
                var updateRequest = new UpdateUserRequest
                {
                    Name = userItem.Name,
                    Email = userItem.Email,
                    PhoneNumber = userItem.PhoneNumber,
                    Role = userItem.Role,
                    Department = userItem.Department,
                    OnboardingStatus = userItem.OnboardingStatus,
                    IsActive = userItem.IsActive
                };

                var result = await _usersRepository.UpdateUserAsync(userItem.Id, updateRequest);
                if (!result)
                {
                    errors.Add($"Failed to update user with ID {userItem.Id}");
                    continue;
                }

                successCount++;
            }
            catch (Exception ex)
            {
                errors.Add($"Error updating user with ID {userItem.Id}: {ex.Message}");
            }
        }

        var message = $"{successCount} users updated successfully.";
        if (errors.Any())
        {
            message += $" Errors: {string.Join("; ", errors)}";
        }

        return message;
    }

    public async Task<string> DeleteUserByIdAsync(string id)
    {
        var result = await _usersRepository.DeleteUserAsync(id);
        if (!result)
        {
            throw new KeyNotFoundException("User not found");
        }
        return $"User deleted successfully";
    }

    public async Task<string> DeleteBulkUsersByRoleAsync(string role)
    {
        var count = await _usersRepository.DeleteUsersByRoleAsync(role);
        return $"{count} users deleted successfully";
    }
}