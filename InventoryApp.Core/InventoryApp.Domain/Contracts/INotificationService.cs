using InventoryApp.Application.Results;
using InventoryApp.Domain.Entities;

namespace InventoryApp.Domain.Contracts;

public interface INotificationService : IGenericService<Notification>
{
    Task<IServiceResultWithData<Notification>> GetNotificationByIdWithInclude(string? include, int id);
    Task<IServiceResultWithData<IEnumerable<Notification>>> GetNotificationsWithInclude(string? include);
}