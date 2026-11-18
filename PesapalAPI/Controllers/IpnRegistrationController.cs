using Microsoft.AspNetCore.Mvc;
using PesapalAPI.Services;

namespace PesapalAPI.Controller;

[ApiController]
[Route("api/registration")]
public class IpnRegistrationController : ControllerBase
{
    private readonly PesapalService _pesapalService;
    private readonly ILogger<IpnRegistrationController> _logger;

    public IpnRegistrationController(PesapalService pesapalService, ILogger<IpnRegistrationController> logger)
    {
        _pesapalService = pesapalService;
        _logger = logger;
    }

    [HttpPost("ipn")]
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


    //Discrepancy from pesapal as it returns an empty list!
    [HttpGet("list")]
    public async Task<IActionResult> GetIpnListAsync()
    {
        try
        {
            var ipnList = await _pesapalService.GetIpnListAsync();
            return Ok(ipnList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get List failed");
            throw;
        }
    }
}