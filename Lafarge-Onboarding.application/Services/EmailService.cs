namespace Lafarge_Onboarding.application.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly HttpClient _httpClient;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger, HttpClient httpClient)
    {
        _configuration = configuration;
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetToken)
    {
        try
        {
            var apiKey = _configuration["ZeptoMail:ApiKey"];
            var baseUrl = _configuration["ZeptoMail:BaseUrl"];
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            var fromName = _configuration["EmailSettings:FromName"];

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(baseUrl) ||
                string.IsNullOrEmpty(fromEmail) || string.IsNullOrEmpty(fromName))
            {
                _logger.LogError("Email configuration is missing");
                return false;
            }

            var resetLink = $"https://yourapp.com/api/auth/reset-password?token={resetToken}&email={toEmail}";

            var emailBody = $@"
                <html>
                <body>
                    <h2>Password Reset Request</h2>
                    <p>You have requested to reset your password for your Lafarge Onboarding account.</p>
                    <p>Please click the link below to reset your password:</p>
                    <a href='{resetLink}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Reset Password</a>
                    <p>This link will expire in 1 hour.</p>
                    <p>If you did not request this password reset, please ignore this email.</p>
                    <br>
                    <p>Best regards,<br>Lafarge Onboarding Team</p>
                </body>
                </html>";

            var requestBody = new
            {
                mail_template_key = "",
                from = new
                {
                    address = fromEmail,
                    name = fromName
                },
                to = new[]
                {
                    new
                    {
                        email_address = new
                        {
                            address = toEmail,
                            name = toEmail
                        }
                    }
                },
                subject = "Password Reset - Lafarge Onboarding",
                htmlbody = emailBody,
                track_clicks = true,
                track_opens = true
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", apiKey);

            var response = await _httpClient.PostAsync($"{baseUrl}/email", content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Password reset email sent successfully to {Email}", toEmail);
                return true;
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to send password reset email to {Email}. Status: {Status}, Response: {Response}",
                    toEmail, response.StatusCode, responseContent);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending password reset email to {Email}", toEmail);
            return false;
        }
    }
}