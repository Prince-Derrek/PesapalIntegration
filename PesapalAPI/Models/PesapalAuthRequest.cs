using System.Text.Json.Serialization;

namespace PesapalAPI.Models;

public class AuthRequest
{
    [JsonPropertyName("consumer_key")]
    public string consumerKey { get; set; } = string.Empty;
    [JsonPropertyName("consumer_secret")]
    public string consumerSecret { get; set; } = string.Empty;
}
