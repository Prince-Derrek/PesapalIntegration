using System.Text.Json.Serialization;

namespace PesapalAPI.Models;

public class PaymentRequest
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? RedirectMode { get; set; }


}