using System.Threading.Tasks;
using PesapalAPI.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace PesapalApi.Services
{
    public class PesapalService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<PesapalService> logger
    )
    {
        public async Task<AuthResponse> AuthenticateAsync()
        {
            // prepare data
            var Key = configuration["Merchant:consumer_key"];
            var Secret = configuration["Merchant:consumer_secret"];
            var authUrl = configuration["Integration:AuthUrl"];

            // construct the payload
            var payload = new
            {
                consumer_key = Key,
                consumer_secret = Secret
            };

            var jsonString = JsonContent.Create(payload);

            // send the request
            using var response = await httpClient.PostAsync(authUrl, jsonString); // using declaration ensures response is disposed

            // ensure success before proceeding
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("POST Auth failed: Status Code: {Status}", response.StatusCode);

                throw new HttpRequestException(
                    $"Server returned {response.StatusCode}"
                );
            }

            var pesapalResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

            if (pesapalResponse is null)
            {
                logger.LogError("Null payload returned from Pesapal Servers");
                throw new InvalidOperationException(
                    "Expected an Authentication response but received a null payload"
                );
            }

            if (pesapalResponse.token is null || pesapalResponse.token is "")
            {
                logger.LogError("No token was returned");
                throw new InvalidOperationException(
                    "No access token was issued"
                );
            }

            if (pesapalResponse.status != "200")
            {
                logger.LogError($"Status : {pesapalResponse.status} was returned");
                throw new InvalidOperationException(
                    "$Pesapal App-level Auth returned status: {pespalResponse.status}"
                );
            }

            return pesapalResponse;

        }
    }
}