using System.Text.Json.Serialization;

namespace PesapalAPI.Models;

public class RegisterIpnResponse
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("created_date")]
    public DateTimeOffset CreatedDate { get; set; }

    [JsonPropertyName("ipn_id")]
    public Guid IpnId { get; set; }

    [JsonPropertyName("notification_type")]
    public int NotificationType { get; set; }

    [JsonPropertyName("ipn_notification_type_description")]
    public string IpnNotificationTypeDescription { get; set; } = string.Empty;

    [JsonPropertyName("ipn_status")]
    public int IpnStatus { get; set; }

    [JsonPropertyName("ipn_status_description")]
    public string IpnStatusDescription { get; set; } = string.Empty;

    [JsonPropertyName("error")]
    public object? Error { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}