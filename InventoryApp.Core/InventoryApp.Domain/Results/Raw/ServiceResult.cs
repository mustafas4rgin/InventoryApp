namespace InventoryApp.Application.Results.Raw;

public class ServiceResult : IServiceResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public ServiceResult(bool success, string message)
    {
        Success = success;
        Message = message;
    }
}
