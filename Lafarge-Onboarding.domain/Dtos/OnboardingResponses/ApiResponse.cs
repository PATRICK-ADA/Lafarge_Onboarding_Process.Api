namespace Lafarge_Onboarding.domain.OnboardingResponses;

public class ApiResponse<T>
{
    public string? Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public string? StatusCode { get; set; } = "200";
    public bool RequestSuccessful { get; set; } = true;
    public DateTime TimeStamp { get; set; }

    public static ApiResponse<T> Success(T data)
    {
        return new ApiResponse<T>
        {
            Data = data,
            Message = "Request Successful",
            StatusCode = "200",
            RequestSuccessful = true,
            TimeStamp = DateTime.UtcNow
        };
    }

    public static ApiResponse<T> Failure(string message = "Request Failed", string statusCode = "400")
    {
        return new ApiResponse<T>
        {
            Message = message,
            Data = default,
            StatusCode = statusCode,
            RequestSuccessful = false,
            TimeStamp = DateTime.UtcNow
        };
    }
}