using System.ComponentModel.DataAnnotations;

namespace SupportManagement.Model;

public class UpdateTicketStatusRequestModel
{
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;
}
