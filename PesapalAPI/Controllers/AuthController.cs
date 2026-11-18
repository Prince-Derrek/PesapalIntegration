using Microsoft.AspNetCore.Mvc;
using PesapalAPI.Services;

namespace PesapalAPI.Controller;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly PesapalService _pesapalService;
    private readonly ILogger<AuthController> _logger;
    public AuthController(PesapalService pesapalService, ILogger<AuthController> logger)
    {
        _pesapalService = pesapalService;
        _logger = logger;
    }

    [HttpPost("token")]
    public async Task<IActionResult> RequestAuth()
    {
        try
        {
            var response = await _pesapalService.AuthenticateAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Authentication request failed.");
            throw;
        }
    }

}

