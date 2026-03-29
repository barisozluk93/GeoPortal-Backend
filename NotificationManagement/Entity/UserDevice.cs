namespace NotificationManagement.Entity
{
    public class UserDevice
    {
        public long Id { get; set; }
        public long UserId { get; set; }

        public string Platform { get; set; } = default!; // ios / android
        public string PushToken { get; set; } = default!;

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime LastSeenAt { get; set; }
    }
}
