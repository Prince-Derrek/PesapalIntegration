using System.Text.Json.Serialization;

namespace PesapalAPI.Models;

public class BillingAddress
{
    [JsonPropertyName("phone_number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [JsonPropertyName("email_address")]
    public string EmailAddress { get; set; } = string.Empty;
}