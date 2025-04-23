namespace InventoryApp.Application.Results.Data;

public class ResultWithData<T> : IServiceResultWithData<T> where T : class
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T Data { get; set; } = null!;
    
    public ResultWithData(bool success, string message, T data)
    {
        Success = success;
        Message = message;
        Data = data;
    }
}