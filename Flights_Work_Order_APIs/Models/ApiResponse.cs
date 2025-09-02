namespace Flights_Work_Order_APIs.Models
{
    /// <summary>
    /// Generic API response wrapper for all endpoints
    /// </summary>
    /// <typeparam name="T">Type of data being returned</typeparam>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public ApiResponse()
        {
        }

        public ApiResponse(bool success, string message, T? data = default)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        /// <summary>
        /// Creates a successful response
        /// </summary>
        public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
        {
            return new ApiResponse<T>(true, message, data);
        }

        /// <summary>
        /// Creates an error response
        /// </summary>
        public static ApiResponse<T> ErrorResponse(string message, T? data = default)
        {
            return new ApiResponse<T>(false, message, data);
        }
    }
}