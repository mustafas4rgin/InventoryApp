using InventoryApp.Application.Results;
using InventoryApp.Domain.Entities;

namespace InventoryApp.Domain.Contracts;

public interface INotificationService : IGenericService<Notification>
{
    Task<IServiceResult> MarkAsReadAsync(int notificationId);
    Task<IServiceResultWithData<Notification>> GetNotificationByIdWithInclude(string? include, int id);
    Task<IServiceResultWithData<IEnumerable<Notification>>> GetNotificationsWithInclude(string? include, string? seaerch);
    Task<IServiceResultWithData<IEnumerable<Notification>>> GetNotificationByUserIdAsync(int userId);
}