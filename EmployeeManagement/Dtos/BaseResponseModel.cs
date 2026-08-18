namespace EmployeeManagement.Dtos;

public class BaseResponseModel<T>
{
    public T? Data { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }


    public static BaseResponseModel<T> SuccessResponse(T data, string message = "Request successful")
    {
        return new BaseResponseModel<T>
        {
            Data = data,
            Success = true,
            Error = string.Empty
        };
    }

    public static BaseResponseModel<T> FailureResponse(string message)
    {
        return new BaseResponseModel<T>
        {
            Data = default,
            Success = false,
            Error = message
        };
    }
}
