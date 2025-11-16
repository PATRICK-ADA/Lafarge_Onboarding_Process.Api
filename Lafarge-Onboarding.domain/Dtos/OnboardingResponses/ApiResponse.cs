namespace Lafarge_Onboarding.domain.Dtos.OnboardingResponses;

public sealed record ApiResponse<T>
{
    public string? Message { get; init; } = string.Empty;
    public T? Result { get; init; }
    public string? StatusCode { get; init; } = "200";
    public bool IsSuccessful { get; init; } = true;
    public DateTime TimeStamp { get; init; }

    public static ApiResponse<T> Success(T data, string message = "Request Successful")
    {
        return new ApiResponse<T>
        {
            Result = data ?? (T)(object)"Success!"!,
            Message = message,
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
            Result = (T)(object)"Failure!"!,
            StatusCode = statusCode,
            IsSuccessful = false,
            TimeStamp = DateTime.UtcNow
        };
    }
}