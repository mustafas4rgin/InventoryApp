using InventoryApp.Domain.Entities;

namespace InventoryApp.Application.DTOs.Create;

public class CreateNotificationDTO
{
    public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int UserId { get; set; }
        public NotificationType Type { get; set; }
        public NotificationStatus Status { get; set; } = NotificationStatus.Unread;
}