namespace OneAPI.Meter.Models
{
    public class Response<T>
    {
        public T? Data { get; set; } = default;

        public string Message { get; set; } = string.Empty;

        public bool Success { get; set; }
    }
}
