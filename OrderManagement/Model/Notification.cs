namespace OrderManagement.Model
{
    public class Notification
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public string Type { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Body { get; set; } = default!;
        public string? TargetUrl { get; set; }
        public string? DataJson { get; set; }

        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }

        public bool IsDeleted { get; set; } = false;


    }
}
