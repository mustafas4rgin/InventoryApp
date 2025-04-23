using InventoryApp.Domain.Entities;

namespace InventoryApp.Application.DTOs.List;

public class NotificationDTO
{
    public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int UserId { get; set; }
        public NotificationType Type { get; set; }
        public NotificationStatus Status { get; set; } = NotificationStatus.Unread;
        //navigation properties
        public UserDTO User { get; set; } = null!;
}