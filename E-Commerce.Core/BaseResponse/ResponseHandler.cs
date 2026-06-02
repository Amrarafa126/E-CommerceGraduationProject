public class ApiResponse<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }
    public List<string> Errors { get; init; } = new();
    public int StatusCode { get; init; }

    public static ApiResponse<T> Ok(T data, string message = "تمت العملية بنجاح.")
        => new() { Success = true, Data = data, Message = message, StatusCode = 200 };

    public static ApiResponse<T> Created(T data, string message = "تم إنشاء المورد بنجاح.")
        => new() { Success = true, Data = data, Message = message, StatusCode = 201 };

    public static ApiResponse<T> Fail(string message, int statusCode = 400, List<string>? errors = null)
        => new() { Success = false, Message = message, StatusCode = statusCode, Errors = errors ?? new() };

    public static ApiResponse<T> NotFound(string message = "المورد غير موجود.")
        => new() { Success = false, Message = message, StatusCode = 404 };

    public static ApiResponse<T> Unauthorized(string message = "غير مصرح بالوصول.")
        => new() { Success = false, Message = message, StatusCode = 401 };

    public static ApiResponse<T> Forbidden(string message = "تم رفض الوصول.")
        => new() { Success = false, Message = message, StatusCode = 403 };

    public static ApiResponse<T> ValidationFail(List<string> errors, string message = "فشل التحقق من البيانات.")
        => new() { Success = false, Message = message, StatusCode = 422, Errors = errors };
}

public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse<object> Ok(string message = "تمت العملية بنجاح.")
        => new() { Success = true, Message = message, StatusCode = 200 };
}
