using EmployeeManagement.Enums;

public class BaseResponseModel<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public ErrorType ErrorType { get; set; } = ErrorType.None;

    public static BaseResponseModel<T> SuccessResponse(T data, string message) =>
        new() { Success = true, Data = data, Message = message };

    public static BaseResponseModel<T> FailureResponse(string message, ErrorType errorType = ErrorType.Validation) =>
        new() { Success = false, Message = message, ErrorType = errorType };
}