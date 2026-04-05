using System.ComponentModel.DataAnnotations;

namespace SupportManagement.Model;

public class ContactFormRequestModel
{
    [Required]
    [MinLength(2)]
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Organization { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string BusinessEmail { get; set; } = string.Empty;

    [Required]
    [MinLength(3)]
    [MaxLength(250)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    [MinLength(10)]
    [MaxLength(5000)]
    public string Message { get; set; } = string.Empty;
}
