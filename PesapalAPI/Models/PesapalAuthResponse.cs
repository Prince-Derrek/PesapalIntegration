namespace PesapalAPI.Models;

public class AuthResponse
{
    public string token { get; set; } = string.Empty;
    public DateTimeOffset expiryDate { get; set; }
    public object? error { get; set; }
    public string status { get; set; } = string.Empty;
    public string message { get; set; } = string.Empty;
}