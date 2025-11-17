namespace Health.Application.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        public object? Errors { get; set; }
        public int StatusCode { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string message = "", int statusCode = 200)
            => new()
            {
                Success = true,
                Data = data,
                Message = message,
                StatusCode = statusCode
            };

        public static ApiResponse<T> ErrorResponse(string error, int statusCode = 400)
    => new()
    {
        Success = false,
        Message = error,
        Errors = new List<string> { error },
        StatusCode = statusCode
    };


        public static ApiResponse<T> ErrorResponse(string message, object errors, int statusCode = 400)
            => new()
            {
                Success = false,
                Message = message,
                Errors = errors,
                StatusCode = statusCode
            };
    }
}
