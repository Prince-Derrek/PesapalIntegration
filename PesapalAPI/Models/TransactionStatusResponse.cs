using System.Text.Json.Serialization;

namespace PesapalAPI.Models;

public class TransactionStatusResponse
{
    [JsonPropertyName("payment_method")]
    public string PaymentMethod { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("created_date")]
    public DateTimeOffset CreatedDate { get; set; }

    [JsonPropertyName("confirmation_code")]
    public string ConfirmationCode { get; set; } = string.Empty;

    [JsonPropertyName("payment_status_description")]
    public string PaymentStatusDescription { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("payment_account")]
    public string PaymentAccount { get; set; } = string.Empty;

    [JsonPropertyName("callback_url")]
    public string CallbackUrl { get; set; } = string.Empty;

    [JsonPropertyName("status_code")]
    public int StatusCode { get; set; }

    [JsonPropertyName("merchant_reference")]
    public string MerchantReference { get; set; } = string.Empty;

    [JsonPropertyName("payment_status_code")]
    public string? PaymentStatusCode { get; set; } = string.Empty;

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonPropertyName("error")]
    public Error? Error { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

}