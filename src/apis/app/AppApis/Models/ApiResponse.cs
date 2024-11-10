namespace Habanerio.Xpnss.Apis.App.AppApis.Models;

public record ApiResponse
{
    public bool IsSuccess { get; set; }

    public string Message { get; set; } = "";

    public ApiResponse()
    {
        IsSuccess = true;
    }

    public static ApiResponse Ok() => new()
    {
        IsSuccess = true
    };

    public static ApiResponse Fail(string errorMessage) => new()
    {
        IsSuccess = false,
        Message = errorMessage
    };
}

public record ApiResponse<T>(T? Data) : ApiResponse
{
    public static ApiResponse<T> Ok(T? data) => new(data)
    {
        IsSuccess = true
    };
}