namespace InventoryApp.Application.Results.Raw;

public class SuccessResult : ServiceResult
{
    public SuccessResult(string message) : base(true,message) {}
}