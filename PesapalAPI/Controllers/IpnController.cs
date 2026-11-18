using Microsoft.AspNetCore.Mvc;

namespace PesapalAPI.Controller;

[ApiController]
[Route("api/ipn")]
public class IpnController : ControllerBase
{
    private readonly ILogger<IpnController> _logger;

    public IpnController(ILogger<IpnController> logger)
    {
        _logger = logger;
    }
    [HttpGet]
    public IActionResult PaymentNotification(string orderTrackingId, string orderNotificationType, string orderMerchantReference)
    {
        _logger.LogInformation("{TrackingId}, {NotificationType}, {MerchantReference}", orderTrackingId, orderNotificationType, orderMerchantReference);
        var payload = new
        {
            OrderNotificationType = orderNotificationType,
            OrderTrackingId = orderTrackingId,
            OrderMerchantReference = orderMerchantReference,
            Status = 200
        };
        return Ok(payload);

    }
}