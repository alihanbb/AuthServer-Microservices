namespace SharedLibrary.Common;

public abstract class BaseResponse
{
    public string Message { get; init; }
    public int StatusCode { get; init; }
    public bool IsSuccess { get; init; }

    protected BaseResponse(string message, int statusCode, bool isSuccess)
    {
        Message = message;
        StatusCode = statusCode;
        IsSuccess = isSuccess;
    }
}

public abstract class BaseResponse<T>
{
    public T? Data { get; init; }
    public string Message { get; init; }
    public int StatusCode { get; init; }
    public bool IsSuccess { get; init; }
    protected BaseResponse(T data, string message, int statusCode, bool isSuccess)
    {
        Data = data;
        Message = message;
        StatusCode = statusCode;
        IsSuccess = isSuccess;
    }
}


public class SuccessResponse<T> : BaseResponse<T>
{
    public SuccessResponse(T data, string message, int statusCode = 200, bool isSuccess = true)
        : base(data, message, statusCode, isSuccess)
    {
    }
}
public class CreatedResponse<T> : BaseResponse<T>
{
    public CreatedResponse(T data, string message, int statusCode = 201, bool isSuccess = true)
        : base(data, message, statusCode, isSuccess)
    {
    }
}
public class ErrorResponse<T> : BaseResponse<T>
{
    public ErrorResponse(T data, string message, int statusCode = 400, bool isSuccess = false)
        : base(data, message, statusCode, isSuccess)
    {
    }
}
public class NotFoundResponse<T> : BaseResponse<T>
{
    public NotFoundResponse(T data = default(T), string message = "Kayýt bulunamadý.", int statusCode = 404, bool isSuccess = false)
        : base(data, message, statusCode, isSuccess)
    {
    }
}
public class UnauthorizedResponse<T> : BaseResponse<T>
{
    public UnauthorizedResponse(T data = default(T), string message = "Yetkiniz bulunmamaktadýr.", int statusCode = 401, bool isSuccess = false)
        : base(data, message, statusCode, isSuccess)
    {
    }
}


public class SuccessResponse : BaseResponse
{
    public SuccessResponse(string message, int statusCode = 200, bool isSuccess = true)
        : base(message, statusCode, isSuccess)
    {
    }
}

public class CreatedResponse : BaseResponse
{
    public CreatedResponse(string message, int statusCode = 201, bool isSuccess = true)
        : base(message, statusCode, isSuccess)
    {
    }
}

public class ErrorResponse : BaseResponse
{
    public ErrorResponse(string message, int statusCode = 400, bool isSuccess = false)
        : base(message, statusCode, isSuccess)
    {
    }
}
public class NotFoundResponse : BaseResponse
{
    public NotFoundResponse(string message = "Kayýt bulunamadý.", int statusCode = 404, bool isSuccess = false)
        : base(message, statusCode, isSuccess)
    {
    }
}
public class UnauthorizedResponse : BaseResponse
{
    public UnauthorizedResponse(string message = "Yetkiniz bulunmamaktadýr.", int statusCode = 401, bool isSuccess = false)
        : base(message, statusCode, isSuccess)
    {
    }
}
