using Microsoft.EntityFrameworkCore.Query;

namespace Lafarge_Onboarding.domain.OnboardingResponses;

public class ApiResponse<T>
{
    public string? Message { get; set; } = string.Empty;
    public T? Result { get; set; }
    public string? StatusCode { get; set; } = "200";
    public bool IsSuccessful { get; set; } = true;
    public DateTime TimeStamp { get; set; }

    public static ApiResponse<T> Success(T data)
    {
        return new ApiResponse<T>
        {
            Result = data,
            Message = "Request Successful",
            StatusCode = "200",
            IsSuccessful = true,
            TimeStamp = DateTime.UtcNow
        };
    }

    public static ApiResponse<T> Failure(string message = "Request Failed", string statusCode = "400")
    {
        return new ApiResponse<T>
        {
            Message = message,
            Result = typeof(T) is null ? (T)(object)"Failure" : default,
            StatusCode = statusCode,
            IsSuccessful = false,
            TimeStamp = DateTime.UtcNow
        };
    }
}