using Microsoft.AspNetCore.Mvc;

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
    public IActionResult PaymentCallback(string orderTrackingId, string orderMerchantReference, string orderNotificationType)
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