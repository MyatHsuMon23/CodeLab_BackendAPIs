namespace Flights_Work_Order_APIs.Models
{
    /// <summary>
    /// Generic API response wrapper for all API endpoints
    /// </summary>
    /// <typeparam name="T">Type of data being returned</typeparam>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        /// <summary>
        /// Creates a successful response with data
        /// </summary>
        public static ApiResponse<T> CreateSuccess(T data, string message = "Operation completed successfully")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        /// <summary>
        /// Creates a failed response with error message
        /// </summary>
        public static ApiResponse<T> CreateError(string message)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default
            };
        }
    }

    /// <summary>
    /// Non-generic API response for endpoints that don't return data
    /// </summary>
    public class ApiResponse : ApiResponse<object>
    {
        /// <summary>
        /// Creates a successful response without data
        /// </summary>
        public static ApiResponse CreateSuccess(string message = "Operation completed successfully")
        {
            return new ApiResponse
            {
                Success = true,
                Message = message,
                Data = null
            };
        }

        /// <summary>
        /// Creates a failed response with error message
        /// </summary>
        public new static ApiResponse CreateError(string message)
        {
            return new ApiResponse
            {
                Success = false,
                Message = message,
                Data = null
            };
        }
    }
}