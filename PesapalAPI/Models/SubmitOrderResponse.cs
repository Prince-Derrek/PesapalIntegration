using System.Text.Json.Serialization;

namespace PesapalAPI.Models;

public class SubmitOrderResponse
{
    [JsonPropertyName("order_tracking_id")]
    public string OrderTrackingId { get; set; } = string.Empty;

    [JsonPropertyName("merchant_reference")]
    public string MerchantReference { get; set; } = string.Empty;

    [JsonPropertyName("redirect_url")]
    public string RedirectUrl { get; set; } = string.Empty;

    [JsonPropertyName("error")]
    public int? Error { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}