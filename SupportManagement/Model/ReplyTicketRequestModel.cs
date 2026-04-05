using System.ComponentModel.DataAnnotations;

namespace SupportManagement.Model;

public class ReplyTicketRequestModel
{
    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string AdminEmail { get; set; } = string.Empty;

    [Required]
    [MinLength(2)]
    [MaxLength(10000)]
    public string Message { get; set; } = string.Empty;
}
