using Microsoft.AspNetCore.Mvc;
using PesapalAPI.Models;
using PesapalAPI.Services;

namespace PesapalAPI.Controller;

[ApiController]
[Route("api/payment")]
public class PaymentController : ControllerBase
{
    private readonly PesapalService _pesapalService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(
        PesapalService pesapalService,
        ILogger<PaymentController> logger)
    {
        _pesapalService = pesapalService;
        _logger = logger;
    }

    [HttpPost("submit")]
    public async Task<IActionResult> SubmitPayment(
        [FromBody] PaymentOrderRequest request)
    {
        var response = await _pesapalService.SubmitOrderAsync(
            request.Amount,
            request.Description,
            request.PhoneNumber,
            request.EmailAddress
        );

        _logger.LogInformation(
            "Payment order submitted. Merchant Reference: {Reference}",
            response.MerchantReference
        );

        return Ok(response);
    }

    [HttpGet("tracking")]
    public async Task<IActionResult> TrackPayment(string orderTrackingId)
    {
        var response = await _pesapalService.GetTransactionStatusAsync(orderTrackingId);

        _logger.LogInformation(
            "Tracking new payment order. Order Reference Id: {Reference}",
            orderTrackingId
        );

        return Ok(response);
    }
}