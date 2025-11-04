namespace Lafarge_Onboarding.application.Services;

public sealed class AuthService : IAuthService
{
    private readonly UserManager<Users> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<Users> userManager,
        RoleManager<Role> roleManager,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> GenerateJwtToken(Users user)
    {
        _logger.LogInformation("Generating JWT token for user: {UserId}", user.Id);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName),
            new Claim("role", user.Role)
        };

        var userRoles = await _userManager.GetRolesAsync(user);
        claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(8),
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        _logger.LogInformation("JWT token generated successfully for user: {UserId}", user.Id);

        return tokenString;
    }

    public async Task<Users?> ValidateUserCredentials(string email, string password)
    {
        _logger.LogInformation("Validating credentials for email: {Email}", email);

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            _logger.LogWarning("User not found for email: {Email}", email);
            return null;
        }

        var result = await _userManager.CheckPasswordAsync(user, password);
        if (!result)
        {
            _logger.LogWarning("Invalid password for user: {UserId}", user.Id);
            return null;
        }

        _logger.LogInformation("Credentials validated successfully for user: {UserId}", user.Id);
        return user;
    }

    public async Task<AuthRegisterResponse> RegisterUserAsync(AuthRegisterRequest request)
    {
        _logger.LogInformation("Registering new user with email: {Email}", request.Email);

        try
        {
            var user = new Users
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Role = request.Role,
                EmailConfirmed = true // For demo purposes
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                _logger.LogError("Failed to create user: {Email}. Errors: {Errors}",
                    request.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                throw new Exception($"User registration failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            // Assign role to user
            var roleResult = await _userManager.AddToRoleAsync(user, request.Role);
            if (!roleResult.Succeeded)
            {
                _logger.LogError("Failed to assign role {Role} to user: {Email}", request.Role, request.Email);
                throw new Exception($"Role assignment failed: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
            }

            _logger.LogInformation("User registered successfully: {UserId}", user.Id);
            return new AuthRegisterResponse { Message = "User registered successfully" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration for email: {Email}", request.Email);
            throw;
        }
    }

    public async Task<AuthLoginResponse?> LoginUserAsync(AuthLoginRequest request)
    {
        _logger.LogInformation("Processing login request for email: {Email}", request.Email);

        try
        {
            var user = await ValidateUserCredentials(request.Email, request.Password);
            if (user == null)
            {
                _logger.LogWarning("Invalid login credentials for email: {Email}", request.Email);
                return null;
            }

            var token = await GenerateJwtToken(user);

            var response = new AuthLoginResponse
            {
                Token = token,
                User = new AuthUserInfo
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role
                }
            };

            _logger.LogInformation("Login successful for user: {UserId}", user.Id);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
            throw;
        }
    }

    public async Task<bool> AssignRoleToUserAsync(string userId, string roleName)
    {
        _logger.LogInformation("Assigning role {Role} to user: {UserId}", roleName, userId);

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", userId);
            return false;
        }

        // Ensure role exists
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            var role = new Role
            {
                Name = roleName,
                Description = $"{roleName} role"
            };
            var roleResult = await _roleManager.CreateAsync(role);
            if (!roleResult.Succeeded)
            {
                _logger.LogError("Failed to create role: {Role}", roleName);
                return false;
            }
        }

        var result = await _userManager.AddToRoleAsync(user, roleName);
        if (!result.Succeeded)
        {
            _logger.LogError("Failed to assign role {Role} to user: {UserId}", roleName, userId);
            return false;
        }

        _logger.LogInformation("Role {Role} assigned successfully to user: {UserId}", roleName, userId);
        return true;
    }
}