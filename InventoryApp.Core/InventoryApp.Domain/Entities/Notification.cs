namespace InventoryApp.Domain.Entities
{
    public class Notification : EntityBase
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int UserId { get; set; }
        public NotificationType Type { get; set; }
        public NotificationStatus Status { get; set; } = NotificationStatus.Unread;
        //navigation properties
        public User User { get; set; } = null!;
        
    }
    public enum NotificationType
    {
        Info,
        Warning,
        Error,
        Success,
        OutOfStock
    }

    public enum NotificationStatus
    {
        Unread,
        Read
    }
}
