namespace NotificationManagement.Model
{
    public class RegisterDeviceRequest
    {
        public string Platform { get; set; } = default!;
        public string PushToken { get; set; } = default!;

        public long UserId {  get; set; }
    }
}
