namespace Application.DTOs
{
    public class ApiResponse<T>
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; } // Permite valores nulos
        public List<string>? Errors { get; set; } // Permite valores nulos

        public ApiResponse(int code, string message, T? data = default, List<string>? errors = null)
        {
            Code = code;
            Message = message;
            Data = data;
            Errors = errors ?? new List<string>();
        }
    }
}