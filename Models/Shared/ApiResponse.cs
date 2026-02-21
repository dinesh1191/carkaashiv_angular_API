namespace carkaashiv_angular_API.Models.Shared
{
    public class ApiResponse<T>
    {      
            public bool Success { get; set; }
            public string Message { get; set; }
            public T? Data { get; set; }

            public ApiResponse(bool success, string message, T? data = default)
            {
                Success = success;
                Message = message;
                Data = data;
            }

            // Convenience static methods
            public static ApiResponse<T> Ok(string message, T? data = default)
                => new(true, message, data);

            public static ApiResponse<T> Fail(string message)
                => new(false, message);
        }
    }
