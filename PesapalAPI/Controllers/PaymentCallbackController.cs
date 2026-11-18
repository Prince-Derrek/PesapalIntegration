using Microsoft.AspNetCore.Mvc;
using PesapalAPI.Models;

namespace PesapalAPI.Controller;

[ApiController]
[Route("api/payment")]
public class PaymentCallbackController : ControllerBase
{
    private readonly ILogger<PaymentCallbackController> _logger;

    public PaymentCallbackController(ILogger<PaymentCallbackController> logger)
    {
        _logger = logger;
    }

    [HttpGet("callback")]
    public IActionResult PaymentCallback([FromQuery] NotificationModel req)
    {
        _logger.LogInformation("{TrackingId}, {NotificationType}, {MerchantReference}", req.OrderTrackingId, req.OrderNotificationType, req.OrderMerchantReference);
        var payload = new NotificationModel
        {
            OrderNotificationType = req.OrderNotificationType,
            OrderTrackingId = req.OrderTrackingId,
            OrderMerchantReference = req.OrderMerchantReference,
        };
        return Ok(payload);
    }
}