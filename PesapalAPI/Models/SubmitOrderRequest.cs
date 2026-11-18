using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PesapalAPI.Models;

public class SubmitOrderRequest
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("callback_url")]
    public string CallbackUrl { get; set; } = string.Empty;

    [JsonPropertyName("notification_id")]
    public string NotificationId { get; set; } = string.Empty;

    [JsonPropertyName("billing_address")]
    [Required]
    public BillingAddress BillingAddress { get; set; } = null!;
}