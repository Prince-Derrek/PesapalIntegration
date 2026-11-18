using Microsoft.AspNetCore.Mvc;
using PesapalAPI.Models;

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
    public IActionResult PaymentNotification([FromQuery] NotificationModel req)
    {
        _logger.LogInformation("{TrackingId}, {NotificationType}, {MerchantReference}", req.OrderTrackingId, req.OrderNotificationType, req.OrderMerchantReference);
        var payload = new
        {
            orderNotificationType = req.OrderNotificationType,
            orderTrackingId = req.OrderTrackingId,
            orderMerchantReference = req.OrderMerchantReference,
            Status = 200
        };
        return Ok(payload);

    }
}