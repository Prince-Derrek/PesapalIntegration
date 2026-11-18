namespace PesapalAPI.Models;

public class NotificationModel
{
    public string OrderTrackingId { get; set; } = string.Empty;
    public string OrderMerchantReference { get; set; } = string.Empty;
    public string OrderNotificationType { get; set; } = string.Empty;
}