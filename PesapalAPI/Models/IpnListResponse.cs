using System.Text.Json.Serialization;

namespace PesapalAPI.Models;

public class IpnListResponse
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("created_date")]
    public DateTimeOffset CreatedDate { get; set; }

    [JsonPropertyName("ipn_id")]
    public Guid IpnId { get; set; }

    [JsonPropertyName("error")]
    public int? Error { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}