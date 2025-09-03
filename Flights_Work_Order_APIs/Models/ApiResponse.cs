namespace Flights_Work_Order_APIs.Models
{
    /// <summary>
    /// Pagination metadata for API responses
    /// </summary>
    public class Pagination
    {
        public int CurrentPage { get; set; }
        public int LastPage { get; set; }
        public int PerPage { get; set; }
        public int Total { get; set; }
    }

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

    /// <summary>
    /// Paginated API response wrapper for endpoints that return paginated data
    /// </summary>
    /// <typeparam name="T">Type of data being returned</typeparam>
    public class PaginatedApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public Pagination? Pagination { get; set; }

        /// <summary>
        /// Creates a successful paginated response with data and pagination info
        /// </summary>
        public static PaginatedApiResponse<T> CreateSuccess(T data, Pagination pagination, string message = "Operation completed successfully")
        {
            return new PaginatedApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Pagination = pagination
            };
        }

        /// <summary>
        /// Creates a failed paginated response with error message
        /// </summary>
        public static PaginatedApiResponse<T> CreateError(string message)
        {
            return new PaginatedApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Pagination = null
            };
        }
    }
}