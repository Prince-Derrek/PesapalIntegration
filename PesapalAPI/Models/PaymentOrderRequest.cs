using System.ComponentModel.DataAnnotations;

namespace PesapalAPI.Models;

public class PaymentOrderRequest
{
    public decimal Amount { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public string? EmailAddress { get; set; }
}