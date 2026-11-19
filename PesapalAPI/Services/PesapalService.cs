using System.Threading.Tasks;
using PesapalAPI.Models;
using System.Net.Http.Json;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Text;

namespace PesapalAPI.Services
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
            if (Key is null || Key is "")
            {
                logger.LogError("Consumer Key is missing");
                throw new InvalidOperationException(
                    "Consumer Key is missing"
                );
            }

            var Secret = configuration["Merchant:consumer_secret"];
            if (Secret is null || Secret is "")
            {
                logger.LogError("Consumer Secret is missing");
                throw new InvalidOperationException(
                    "Consumer Secret is missing"
                );
            }

            var authUrl = configuration["Integration:AuthUrl"];
            if (authUrl is null || authUrl is "")
            {
                logger.LogError("Authentication Url is missing");
                throw new InvalidOperationException(
                    "Authentication Url is missing"
                );
            }

            // construct the payload
            var payload = new PesapalAuthRequest
            {
                consumerKey = Key,
                consumerSecret = Secret
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

            var token = pesapalResponse.token;

            return pesapalResponse;

        }

        public async Task<RegisterIpnResponse> RegisterIpnAsync()
        {
            var authToken = await AuthenticateAsync();
            var bearerToken = authToken.token;

            var ipnReg = configuration["Integration:IpnReg"];
            if (string.IsNullOrWhiteSpace(ipnReg))
            {
                throw new InvalidOperationException(
                    "Missing Registration url");
            }

            var ipnUrl = configuration["Integration:IpnUrl"];
            if (string.IsNullOrWhiteSpace(ipnUrl))
            {
                throw new InvalidOperationException(
                    "Missing callback url"
                );
            }

            var payload = new RegisterIpnRequest
            {
                Url = ipnUrl,
                IpnNotificationType = "GET"
            };

            string json = JsonSerializer.Serialize(payload);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var request = new HttpRequestMessage(HttpMethod.Post, ipnReg)
            {
                Content = content
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Error occured when the post request was sent Status: {}", response.StatusCode);
                throw new InvalidOperationException(
                    "Register Ipn request failed with response code"
                );
            }

            var registrationResponse = await response.Content.ReadFromJsonAsync<RegisterIpnResponse>();

            if (registrationResponse is null)
            {
                logger.LogError("Error Occured. Response was null");
                throw new InvalidOperationException(
                    "Expected an Authentication response but received a null payload"
                );
            }

            if (registrationResponse.Status != "200")
            {
                logger.LogError("Registration Failed with status code: {Status}", registrationResponse.Status);
                throw new InvalidOperationException(
                    "Registration failed.");
            }

            return registrationResponse;

        }

        public async Task<List<IpnListResponse>> GetIpnListAsync()
        {
            var authToken = await AuthenticateAsync();
            var bearerToken = authToken.token;

            var getIpnUrl = configuration["Integration:GetIpn"];
            if (string.IsNullOrWhiteSpace(getIpnUrl))
            {
                logger.LogError("The Get IPN Url is missing");
                throw new InvalidOperationException(
                    "The Get IPN Url is missing"
                );
            }

            using var request = new HttpRequestMessage(HttpMethod.Get, getIpnUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Error occured when the GET request was sent Status: {StatusCode}", response.StatusCode);
                throw new InvalidOperationException(
                    "Get IPN list request failed"
                );
            }

            var ipnListResponse = await response.Content.ReadFromJsonAsync<List<IpnListResponse>>();
            if (ipnListResponse is null)
            {
                logger.LogError("Pesapal returned a null response");
                throw new InvalidOperationException(
                    "An Unexpected error occured. Received a null response"
                );
            }

            if (ipnListResponse.Count == 0)
            {
                logger.LogInformation("No Ipn registrations were founf");
                return ipnListResponse;
            }

            return ipnListResponse;
        }

        public async Task<SubmitOrderResponse> SubmitOrderAsync(decimal amount, string description, string? phoneNumber, string? emailAddress)
        {
            // Input Validation
            if (
                string.IsNullOrWhiteSpace(phoneNumber)
                &&
                string.IsNullOrWhiteSpace(emailAddress)
            )
            {
                logger.LogError("Both Phone Number and Email Address were not provided");
                throw new InvalidOperationException(
                    "Both Phone Number and Email Address were not provided"
                );
            }

            if (amount is <= 0)
            {
                logger.LogError(
                    "Amount provided: {Amount} is invalid for a transaction", amount
                    );
                throw new InvalidOperationException(
                    "The Amount provided is incalid for a transaction"
                );
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                logger.LogError("Order Description was not provided");
                throw new InvalidOperationException(
                    "Order Description was not provided"
                );
            }

            var authToken = await AuthenticateAsync();
            var bearerToken = authToken.token;

            //Prepare data
            var orderUrl = configuration["Integration:OrderUrl"];
            if (string.IsNullOrWhiteSpace(orderUrl))
            {
                logger.LogError("Order Url is missing");
                throw new InvalidOperationException(
                    "Order Url is missing"
                );
            }

            var notificationId = configuration["Integration:IpnId"];
            if (string.IsNullOrWhiteSpace(notificationId))
            {
                logger.LogError("Notification Id is missing");
                throw new InvalidOperationException(
                    "Notification Id is missing"
                );
            }

            var callbackUrl = configuration["Integration:CallbackUrl"];
            if (string.IsNullOrWhiteSpace(callbackUrl))
            {
                logger.LogError("Callback Url is missing");
                throw new InvalidOperationException(
                    "Callback Url is missing"
                );
            }

            var billingAddress = new BillingAddress
            {
                PhoneNumber = phoneNumber,
                EmailAddress = emailAddress
            };

            var payload = new SubmitOrderRequest
            {
                Id = Guid.NewGuid().ToString(),
                Currency = "KES",
                Amount = amount,
                Description = description,
                CallbackUrl = callbackUrl,
                NotificationId = notificationId,
                BillingAddress = billingAddress
            };

            string json = JsonSerializer.Serialize(payload);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var request = new HttpRequestMessage(HttpMethod.Post, orderUrl)
            {
                Content = content
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Error occured when the Order request was sent Status: {StatusCode} for Order Id {OrderId}", response.StatusCode, payload.Id);
                throw new InvalidOperationException(
                    $"Submit order request failed || OrderID {payload.Id}"
                );
            }

            var orderResponse = await response.Content.ReadFromJsonAsync<SubmitOrderResponse>();
            if (orderResponse is null)
            {
                logger.LogError("Pesapal returned a null response");
                throw new InvalidOperationException(
                    "An Unexpected error occured. Received a null response"
                );
            }

            if (orderResponse.Status != "200")
            {
                var errorMessage = orderResponse.Error;
                logger.LogError("Order failed with status: {StatusCode} || {ErrorMessage}", orderResponse.Status, errorMessage);
                throw new InvalidOperationException(
                    "Error Occured. Order Failed"
                );
            }

            return orderResponse;

        }

        public async Task<TransactionStatusResponse> GetTransactionStatusAsync(string orderTrackingId)
        {
            if (string.IsNullOrWhiteSpace(orderTrackingId))
            {
                logger.LogError("Order Tracking Id is missing");
                throw new InvalidOperationException(
                    "Order Tracking Id is missing"
                );
            }
            var transactionStatusUrl = configuration["Integration:TransactionStatus"];
            if (string.IsNullOrWhiteSpace(transactionStatusUrl))
            {
                logger.LogError("Get Transaction Status Url is missing");
                throw new InvalidOperationException(
                    "Get Transaction Status Url is missing"
                );
            }

            var authToken = await AuthenticateAsync();
            var bearerToken = authToken.token;

            var requestUrl = $"{transactionStatusUrl}?orderTrackingId={Uri.EscapeDataString(orderTrackingId)}";

            using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Error occured when the Order Tracking request was sent || Status: {StatusCode} for Order Tracking Id {OrderId}", response.StatusCode, orderTrackingId);
                throw new InvalidOperationException(
                    $"Order tracking request failed || Order Tracking Id {orderTrackingId}"
                );
            }

            var orderTrackingResponse = await response.Content.ReadFromJsonAsync<TransactionStatusResponse>();
            if (orderTrackingResponse is null)
            {
                logger.LogError("Pesapal returned a null response");
                throw new InvalidOperationException(
                    "An Unexpected error occured. Received a null response"
                );
            }

            return orderTrackingResponse;

        }
    }
}