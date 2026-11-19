using System.Text.Json.Serialization;

namespace PesapalAPI.Models;

public class PesapalError
{
    [JsonPropertyName("error_type")]
    public string? ErrorType { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public string? Code { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string? Message { get; set; } = string.Empty;

    [JsonPropertyName("call_back_url")]
    public string? CallbackUrl { get; set; } = string.Empty;
}