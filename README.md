# Pesapal Integration API

Pesapal Integration API is an ASP.NET Core Web API that provides a REST wrapper around the Pesapal 3.0 payment APIs. It handles merchant authentication, IPN registration and discovery, payment-order submission, payment callbacks, and transaction-status lookup.

## Features

- Requests and returns a Pesapal bearer token.
- Registers a merchant IPN notification endpoint.
- Lists registered IPN endpoints.
- Submits KES payment orders through Pesapal.
- Accepts Pesapal payment callback and IPN query parameters.
- Retrieves transaction status using a Pesapal order-tracking ID.
- Exposes Swagger UI in the Development environment.

## Technology

- .NET 10
- ASP.NET Core Web API
- `HttpClient` with a typed `PesapalService`
- Newtonsoft.Json 13.0.4
- Swashbuckle Swagger/OpenAPI 11.0.1

## Architecture

```text
HTTP client
    |
    v
API controllers
    |
    v
PesapalService
    |
    v
Pesapal 3.0 APIs
```

### Components

| Component | Responsibility |
|---|---|
| `Program.cs` | Configures controllers, Swagger, HTTPS redirection, the typed HTTP client, and the application pipeline |
| `Controllers/AuthController.cs` | Requests a Pesapal access token |
| `Controllers/IpnRegistrationController.cs` | Registers and lists IPN notification endpoints |
| `Controllers/PaymentController.cs` | Submits payments and retrieves transaction status |
| `Controllers/PaymentCallbackController.cs` | Receives the browser redirect from Pesapal |
| `Controllers/IpnController.cs` | Receives Pesapal server notifications |
| `Services/PesapalService.cs` | Performs authenticated Pesapal API operations |
| `Models/` | Defines request, response, callback, and IPN payloads |

## Prerequisites

- .NET 10 SDK
- A Pesapal merchant account
- Pesapal sandbox or production credentials
- A publicly reachable HTTPS callback/IPN URL for end-to-end callback testing

## Configuration

Do not commit merchant credentials or deployment-specific URLs. Supply the following values through environment variables, the user-secrets store, or a deployment secret provider.

The application reads these configuration keys:

| Key | Purpose | Example placeholder |
|---|---|---|
| `Merchant:consumer_key` | Pesapal merchant consumer key | `<PESAPAL_CONSUMER_KEY>` |
| `Merchant:consumer_secret` | Pesapal merchant consumer secret | `<PESAPAL_CONSUMER_SECRET>` |
| `Integration:BaseUrl` | Pesapal API base URL | `<PESAPAL_BASE_URL>` |
| `Integration:AuthUrl` | Access-token endpoint | `<PESAPAL_AUTH_URL>` |
| `Integration:OrderUrl` | Submit-order endpoint | `<PESAPAL_ORDER_URL>` |
| `Integration:IpnUrl` | Public server-notification URL registered with Pesapal | `<PUBLIC_IPN_URL>` |
| `Integration:IpnReg` | IPN-registration endpoint | `<PESAPAL_IPN_REGISTRATION_URL>` |
| `Integration:GetIpn` | IPN-list endpoint | `<PESAPAL_IPN_LIST_URL>` |
| `Integration:IpnId` | Registered IPN notification ID | `<PESAPAL_IPN_ID>` |
| `Integration:CallbackUrl` | Browser callback URL sent with submitted orders | `<PUBLIC_PAYMENT_CALLBACK_URL>` |
| `Integration:TransactionStatus` | Transaction-status endpoint | `<PESAPAL_TRANSACTION_STATUS_URL>` |

For local development, initialize User Secrets and set the keys using the ASP.NET Core configuration provider:

```powershell
cd .\PesapalAPI
dotnet user-secrets init
dotnet user-secrets set "Merchant:consumer_key" "<PESAPAL_CONSUMER_KEY>"
dotnet user-secrets set "Merchant:consumer_secret" "<PESAPAL_CONSUMER_SECRET>"
dotnet user-secrets set "Integration:BaseUrl" "<PESAPAL_BASE_URL>"
dotnet user-secrets set "Integration:AuthUrl" "<PESAPAL_AUTH_URL>"
dotnet user-secrets set "Integration:OrderUrl" "<PESAPAL_ORDER_URL>"
dotnet user-secrets set "Integration:IpnUrl" "<PUBLIC_IPN_URL>"
dotnet user-secrets set "Integration:IpnReg" "<PESAPAL_IPN_REGISTRATION_URL>"
dotnet user-secrets set "Integration:GetIpn" "<PESAPAL_IPN_LIST_URL>"
dotnet user-secrets set "Integration:IpnId" "<PESAPAL_IPN_ID>"
dotnet user-secrets set "Integration:CallbackUrl" "<PUBLIC_PAYMENT_CALLBACK_URL>"
dotnet user-secrets set "Integration:TransactionStatus" "<PESAPAL_TRANSACTION_STATUS_URL>"
```

Environment variables use double-underscore hierarchy separators, for example:

```powershell
$env:Merchant__consumer_key = "<PESAPAL_CONSUMER_KEY>"
$env:Merchant__consumer_secret = "<PESAPAL_CONSUMER_SECRET>"
$env:Integration__BaseUrl = "<PESAPAL_BASE_URL>"
```

## Build and Run

Restore, build, and run the API:

```powershell
cd .\PesapalAPI
dotnet restore
dotnet build
dotnet run
```

The launch profiles expose:

- HTTP: `Your specific profile in launchsettings`
- HTTPS: `Your specific profile in launchsettings`

Swagger UI is enabled for the Development environment at `/swagger`.

## API Endpoints

All local endpoints below are relative to the running API.

### Authentication

```http
POST /api/auth/token
```

Returns the Pesapal authentication response containing the bearer token and expiry details.

### IPN Registration

```http
POST /api/registration/ipn
```

Registers `Integration:IpnUrl` with Pesapal using a `GET` IPN notification type.

```http
GET /api/registration/list
```

Returns the IPNs currently visible from Pesapal.

### Submit a Payment

```http
POST /api/payment/submit
Content-Type: application/json

{
  "amount": 100.00,
  "description": "<ORDER_DESCRIPTION>",
  "phoneNumber": "<PHONE_NUMBER>",
  "emailAddress": "<EMAIL_ADDRESS>"
}
```

The amount must be greater than zero, the description is required, and at least one of `phoneNumber` or `emailAddress` must be supplied.

The service creates a GUID merchant reference and submits the order in Kenyan shillings (`KES`). A successful response includes the Pesapal order-tracking ID, merchant reference, redirect URL, and status.

### Track a Payment

```http
GET /api/payment/tracking?orderTrackingId=<ORDER_TRACKING_ID>
```

Returns the current Pesapal transaction status and payment details.

### Browser Callback

```http
GET /api/payment/callback?OrderTrackingId=<TRACKING_ID>&OrderNotificationType=<NOTIFICATION_TYPE>&OrderMerchantReference=<MERCHANT_REFERENCE>
```

Returns the callback parameters received from Pesapal.

### IPN Notification

```http
GET /api/ipn?OrderTrackingId=<TRACKING_ID>&OrderNotificationType=<NOTIFICATION_TYPE>&OrderMerchantReference=<MERCHANT_REFERENCE>
```

Logs and acknowledges the server notification with a status of `200`.

## Payment Flow

1. The client calls `POST /api/payment/submit`.
2. `PesapalService` authenticates with Pesapal and obtains a bearer token.
3. The service submits an order containing the callback URL, IPN ID, description, amount, currency, and optional billing details.
4. Pesapal returns an order-tracking ID and redirect URL.
5. The customer is redirected to the Pesapal redirect URL.
6. Pesapal calls the configured callback and IPN URLs.
7. The client or backend can call `GET /api/payment/tracking` with the order-tracking ID to verify final status.

## Security Notes

- Never commit actual consumer keys, consumer secrets, bearer tokens, IPN IDs, or public callback URLs.
- `appsettings.Development.json` may contain local-only values and must not be committed.
- Replace all `<PLACEHOLDER>` values before running the integration.
- Ensure public callback and IPN endpoints use HTTPS in deployed environments.
- Add authentication and authorization to public APIs before exposing this wrapper outside development.
- Validate and reconcile webhook results through the transaction-status endpoint rather than treating a notification as proof of settlement.

## Repository Layout

```text
PesapalAPI/
├── Controllers/
├── Models/
├── Properties/
├── Services/
├── appsettings.json
├── appsettings.Development.json
├── PesapalAPI.csproj
├── PesapalAPI.http
└── Program.cs
```
