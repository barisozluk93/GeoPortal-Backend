namespace SupportManagement.Enums;

public static class TicketStatus
{
    public const string New = "New";
    public const string WaitingForAdmin = "WaitingForAdmin";
    public const string WaitingForCustomer = "WaitingForCustomer";
    public const string CustomerReplied = "CustomerReplied";
    public const string Closed = "Closed";
    public const string Spam = "Spam";
}
