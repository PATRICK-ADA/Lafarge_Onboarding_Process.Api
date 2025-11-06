
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
            Department = u.Department,
            CreatedAt = u.CreatedAt
        });

        return new PaginatedResponse<GetUserResponse>
        {
            Data = userResponses,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ApiResponse<string>> UploadBulkUsersAsync(UploadBulkUsersRequests request)
    {
        var errors = new List<string>();
        var successCount = 0;

        foreach (var userRequest in request.Users)
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
                    FirstName = userRequest.Name.Split(' ').FirstOrDefault() ?? "",
                    LastName = string.Join(" ", userRequest.Name.Split(' ').Skip(1)),
                    PhoneNumber = userRequest.PhoneNumber,
                    Role = userRequest.Role ?? UserRoles.LocalHire,
                    Department = userRequest.Department,
                    EmailConfirmed = true 
                };

               
                var tempPassword = Guid.NewGuid().ToString("N").Substring(0, 8) + "Temp!";

                var result = await _userManager.CreateAsync(user, tempPassword);
                if (!result.Succeeded)
                {
                    errors.Add($"Failed to create user {userRequest.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    continue;
                }

                // Assign role
                var roleName = userRequest.Role ?? UserRoles.LocalHire;
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

        return ApiResponse<string>.Success(message, message);
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
            Department = u.Department,
            CreatedAt = u.CreatedAt
        });

        return new PaginatedResponse<GetUserResponse>
        {
            Data = userResponses,
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
            Department = u.Department,
            CreatedAt = u.CreatedAt
        });

        return new PaginatedResponse<GetUserResponse>
        {
            Data = userResponses,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ApiResponse<GetUserResponse>> GetUserByIdAsync(string id)
    {
        var user = await _usersRepository.GetUserByIdAsync(id);
        if (user == null)
        {
            return ApiResponse<GetUserResponse>.Failure("User not found");
        }

        var userResponse = new GetUserResponse
        {
            Id = user.Id,
            Name = $"{user.FirstName} {user.LastName}",
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
            Department = user.Department,
            CreatedAt = user.CreatedAt
        };

        return ApiResponse<GetUserResponse>.Success(userResponse, "User retrieved successfully");
    }

    public async Task<ApiResponse<string>> UpdateUserByIdAsync(string id, UpdateUserRequest request)
    {
        var result = await _usersRepository.UpdateUserAsync(id, request);
        if (!result)
        {
            return ApiResponse<string>.Failure("User not found");
        }

        return ApiResponse<string>.Success("User updated successfully", "User updated successfully");
    }

    public async Task<ApiResponse<string>> UpdateBulkUsersByRoleAsync(UpdateBulkUsersRequest request)
    {
        var count = await _usersRepository.UpdateUsersByRoleAsync(request.Role, request);
        return ApiResponse<string>.Success($"{count} users updated successfully", $"{count} users updated successfully");
    }

    public async Task<ApiResponse<string>> DeleteUserByIdAsync(string id)
    {
        var result = await _usersRepository.DeleteUserAsync(id);
        if (!result)
        {
            return ApiResponse<string>.Failure("User not found");
        }

        return ApiResponse<string>.Success("User deleted successfully", "User deleted successfully");
    }

    public async Task<ApiResponse<string>> DeleteBulkUsersByRoleAsync(string role)
    {
        var count = await _usersRepository.DeleteUsersByRoleAsync(role);
        return ApiResponse<string>.Success($"{count} users deleted successfully", $"{count} users deleted successfully");
    }
}