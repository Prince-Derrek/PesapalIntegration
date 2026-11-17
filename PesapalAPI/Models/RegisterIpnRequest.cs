using System.Text.Json.Serialization;

namespace PesapalAPI.Models;

public class RegisterIpnRequest
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("ipn_notification_type")]
    public string IpnNotificationType { get; set; } = string.Empty;
}