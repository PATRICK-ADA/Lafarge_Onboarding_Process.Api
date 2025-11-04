namespace Lafarge_Onboarding.domain.OnboardingResponses;

public class ApiResponse<T>
{
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public string StatusCode { get; set; } = "200";

    public static ApiResponse<T> Success(T data, string message = "Operation successful")
    {
        return new ApiResponse<T>
        {
            Message = message,
            Data = data,
            StatusCode = "200"
        };
    }

    public static ApiResponse<T> Failure(string message, string statusCode = "400")
    {
        return new ApiResponse<T>
        {
            Message = message,
            Data = default,
            StatusCode = statusCode
        };
    }
}