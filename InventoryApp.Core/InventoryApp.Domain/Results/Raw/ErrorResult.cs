namespace InventoryApp.Application.Results.Raw;

public class ErrorResult : ServiceResult
{
    public ErrorResult(string message) : base(false,message) {}
}