using Microsoft.AspNetCore.Mvc;
using PesapalAPI.Services;

namespace PesapalAPI.Controller;

[ApiController]
[Route("api/register/ipn")]
public class IpnRegistrationController : ControllerBase
{
    private readonly PesapalService _pesapalService;
    private readonly ILogger<IpnRegistrationController> _logger;

    public IpnRegistrationController(PesapalService pesapalService, ILogger<IpnRegistrationController> logger)
    {
        _pesapalService = pesapalService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> RegisterIpnAsync()
    {
        try
        {
            var response = await _pesapalService.RegisterIpnAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "IPN Registration failed");
            throw;
        }
    }
}