using InventoryApp.Application.Results.Data;

namespace InventoryApp.Core.Results.Data;

public class SuccessResultWithData<T> : ResultWithData<T> where T : class
{
    public SuccessResultWithData(string message, T data) : base(true,message,data) {}
}