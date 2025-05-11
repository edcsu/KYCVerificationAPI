# 🔏 KYC Verification API

A simple yet functional KYC (Know Your Customer) verification REST API that is built using **.NET 9**.
It is designed to simulate the process of validating a user's identity against a national ID system. 
The project highlights my ability to solve real-world problems through [vertical slice architecture](https://blog.ndepend.com/vertical-slice-architecture-in-asp-net-core/).

## 📑 Table of Contents

- [Why This Feature](#-why-this-feature)
- [Prerequisites](#-prerequisites)
- [Getting Started](#-getting-started)
- [API Endpoints](#api-endpoints)
   - [Verifications](#-verifications)
   - [Sample Requests](#-sample-requests)
- [Disclaimer](#-disclaimer)
- [Project Structure](#-project-structure)
- [Running Tests](#-running-tests)
- [Docker Support](#-docker-support)
- [HTTP Status Codes](#-http-status-codes)
- [Rate Limiting](#-rate-limiting)
- [Logging](#-logging)
- [Background Jobs with Hangfire](#-background-jobs-with-hangfire)
   - [Dashboard Access](#dashboard-access)
   - [Security](#security)

## 🚀 Why This Feature

In many financial and identity-sensitive applications, 
verifying a user’s national ID is a critical step. 
This API mimics such a scenario, integrating:

- ✅ Clean separation of concerns with vertical slice architecture
- ✅ Secured REST API using JWT tokens
- ✅ Verify customer identity using a mock external service
- 📡 RESTful API fully documented with [Scalar UI](https://guides.scalar.com/scalar/scalar-api-references/net-integration)
- ✅ Unit tests using xUnit and Moq
- ✅ Integration tests using xUnit and test containers
- ✅ Background jobs to handle long-running tasks
- ✅ Error handling and retry logic
- ✅ Robust input validation
- ✅ OpenTelemetry ready
- ✅ Admin restricted endpoint to generate compliance reports
- 🐳 Optional Docker setup


## 📌 Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Postgres Database](https://www.docker.com/blog/how-to-use-the-postgres-docker-official-image/)
>  To quickly set it up locally:
>``` bash
>  docker run --name some-postgres \ 
>  -e POSTGRES_PASSWORD=mysecretpassword -d postgres
>```
- An IDE ([Visual Studio](https://visualstudio.microsoft.com/downloads), [Rider](https://www.jetbrains.com/rider/download), or [VS Code](https://code.visualstudio.com/download))

## 🛠️ Getting Started

1. Clone the repository
    ```bash
    git clone https://github.com/edcsu/KYCVerificationAPI.git
    
    cd KycVerificationApi
    
    dotnet run
    ```
> Ensure to check out in the `main` branch
> `git checkout main`

2. Install dependencies
    ```bash
     dotnet restore
    ```
 
3. Update your settings in the `appsettings.Development.json` to be in this structure below and replace values to fit your setup.
    ```json lines
      {
        "Logging": {
            "LogLevel": {
                "Default": "Information",
                "Microsoft.AspNetCore": "Warning"
            }
       },
       "Serilog": {
        "MinimumLevel": {
            "Default": "Information"
        },
        "WriteTo": [
            {
                "Name": "Console",
                "outputTemplate": "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}"
            },
            {
                "Name": "File",
                "outputTemplate": "[{Timestamp:HH:mm:ss} {Level}] [{SourceContext}] ({Application}/{MachineName}) {Message}{NewLine}{Exception}",
                "Args": {
                    "path": "Logs/applog.log",
                    "rollingInterval": "Hour",
                    "retainedFileCountLimit": 5000
                }
            }
        ]
        },
        "ConnectionStrings": {
         "DefaultConnection": "Server=localhost;Port=5432;Database=kyc_db;Username=postgres;Password=password"
        },
        "OtelConfing": {
            "Endpoint": "http://localhost:4317",
            "Enabled": false
        },
        "Jwt" : {
            "Key" : "SuperSecretToken2025ForYouAreGoingToProsper",
            "Issuer" : "https://auth.uverify.com",
            "Audience" : "https://kyc.uverify.com"
        },
        "RateLimit" : {
          "PermitLimit" : 1,
          "Window" : 5,
          "QueueLimit" : 5
        }
   }
    ``` 
   
4. Run the application
 ```bash 
   dotnet run
 ```
> The API will be available at `https://localhost:7174` 
> or `http://localhost:5160`
> By default, it opens in the API documentation link.

## API Endpoints

> The API [https documentation can be accessed here](https://localhost:7174) while
> the [http documentation is here](http://localhost:5160)

A summary of the endpoints is shown below

### 🔎 Verifications

| Method | Endpoint                    | Description                                         | Auth Required  |
|--------|-----------------------------|-----------------------------------------------------|----------------|
| POST   | `/api/auth/token`           | Generate an access token                            | No             |
| POST   | `/api/verifications`        | Create new verification                             | Yes            |
| GET    | `/api/verifications/{id}`   | Get verification by ID                              | Yes            |
| GET    | `/api/verifications`        | Returns the list of the verifications that you made | Yes            |
| GET    | `/api/verifications/report` | Returns a compliance report (For admins only)       | Yes            |

### 📝 Sample Requests

> you need an access token first to access the verification endpoints

#### 🔒 Get Access token

The API uses JWT Bearer authentication.
Include the token in the Authorization header.

```http
Authorization: Bearer your-token-here
```

For this showcase, the token is generated as shown below:

1. Make a request to the token endpoint.
There should be at least one custom claim for `client:true` to use
the verification endpoints. This mimics how I set up clients to use the solution.

`Token endpoints: https://localhost:7174/api/auth/token or https://localhost:5160/api/auth/token`

The request body for client use
```json lines
{
  "userId": "2032f3c8-ecc3-4205-94d1-5b05d2ea7c65",
  "email": "test@example.com",
  "customClaims": {
    "client": true
  }
}
```

```curl
   curl https://localhost:7174/api/auth/token \
   --request POST \
   --header 'Content-Type: application/json' \
   --data '{
      "userId": "2032f3c8-ecc3-4205-94d1-5b05d2ea7c65",
      "email": "test@example.com",
      "customClaims": {
        "client": true
      }
   }'
```
>💼 Admins access all endpoints, and 
> you need an access token with admin claim to generate
> the compliance report
>```json5
> // The token request body for admins
>{
>  "userId": "964e69e2-2a02-4ea1-934c-c75fa7c87045",
>  "email": "admin@uverify.com",
>  "customClaims": {
>    "admin": true
>  }
>}
>```
>```curl
>curl https://localhost:7174/api/auth/token \
>--request POST \
>--header 'Content-Type: application/json' \
>--data '{
>   "userId": "2032f3c8-ecc3-4205-94d1-5b05d2ea7c65",
>   "email": "admin@uverify.com",
>   "customClaims": {
>       "admin": true
>   }
>}'
>```

#### Create Verification

>http POST /api/verifications Content-Type: application/json
>Authorization: Bearer {your-token}

```json lines
{
  "firstName": "Kalele",
  "givenName": "Justice",
  "dateOfBirth": "1974-02-12",
  "nin": "12345678901234",
  "cardNumber": "123456789"
}
```

#### Get Verification

> http GET /api/verifications/{id} Authorization: Bearer {your-token}

#### Get Verification history
> http GET /api/verifications Authorization: Bearer {your-token}

##### query parameters

| Parameter                | Type         | Description                                           |
|--------------------------|--------------|-------------------------------------------------------|
| TransactionId            | `Guid`       | Unique identifier of the verification                 |
| Status                   | `string`     | Status of the Verification                            |
| KycStatus                | `bool`       | KYC Status of the Verification                        |
| NameAsPerIdMatches       | `bool`       | Match result of the names                             |
| NinAsPerIdMatches        | `bool`       | Match result of the NIN                               |
| CardNumberAsPerIdMatches | `bool`       | Match result of the card number                       |
| DateOfBirthMatches       | `bool`       | Match result of the date of birth                     |
| DateOfBirth              | `DateOnly`   | Date of birth of the holder on their National ID card |
| FirstName                | `string`     | First name of the holder on their National ID card    |
| GivenName                | `string`     | Given name of the holder on their National ID card    |
| CardNumber               | `string`     | Card number of the holder on their National ID card   |
| Nin                      | `string`     | NIN of the holder on their National ID card           |
| Page                     | `int`        | The page to view                                      |
| PageSize                 | `int`        | The number of items per page                          |
| From                     | `DateOnly`   | The start date of the date range                      |
| To                       | `DateOnly`   | The end date of the date range                        |

## ⚠️ Disclaimer
This is a demo project.
The mock external service does not represent a real ID verification provider.
It shows the interaction that can be there. 
A few scenarios are shown when the external API call is:
- ✅ successful
- ❌ fails
- ❗️returns an error

## 📁 Project Structure

```
KYCVerificationAPI/ 
├── KYCVerificationAPI/ 
│ │ Dependencies
│ ├── Properties/ 
│ ├── wwwroot/ 
│ ├── Core/ 
│ │ ├── Exceptions/ 
│ │ ├── Extensions/ 
│ │ ├── Filters/ 
│ │ ├── Helpers/ 
│ │ ├── Middleware/ 
│ │ ├── ApiConstants
│ │ ├── BaseModel
│ │ ├── JwtConfig
│ │ ├── OtelConfing
│ │ ├── RateLimitConfig
│ │ ├── JwtConfig
│ │ └── VerificationStatus 
│ ├── Data/ 
│ │ ├── Entities/ 
│ │ ├── Repositories/ 
│ │ ├── AppDbContext 
│ │ └── Context 
│ ├── Features/ 
│ │ ├── Auth/ 
│ │ │  ├── Controllers/ 
│ │ │  ├── Requests/ 
│ │ │  ├── Responses/ 
│ │ │  └── Validators/ 
│ │ ├── Scheduler/ 
│ │ │  └── Services/ 
│ │ ├── Vendors/  
│ │ │  ├── Requests/ 
│ │ │  ├── Responses/ 
│ │ │  └── Services/ 
│ │ ├── Verifications/
│ │ │  ├── Controllers/ 
│ │ │  ├── Mappings/ 
│ │ │  ├── Requests/ 
│ │ │  ├── Services/ 
│ │ │  └── Validators/  
│ ├── Logs/ 
│ └── Migrations/ 
├── KYCVerificationAPI.Tests/ 
│ │ Dependencies
│ └── Repositories/ 
│ │ └──VerificationRepositoryTests
├── KYCVerificationAPI.IntegrationTests
│ ├── Dependencies
│ ├── DatabaseFixture
│ ├── IntegrationHelpers
│ └── VerificationRepositoryTests
├── Dockerfile 
└── README.md
```

## ⛓️‍💥 Running Tests

There are two test projects:
1. Unit tests
2. Integration tests

- Execute the test suite using:
```bash 
dotnet test
```

- For specific test projects:
```bash 
dotnet test ./tests/KYCVerificationAPI.IntegrationTests/KYCVerificationAPI.IntegrationTests.csproj
```
```bash
dotnet test ./tests/KYCVerificationAPI.Tests/KYCVerificationAPI.Tests.csproj
```
 
## 🐳 Docker Support

### 🪏 Build the Docker image

```bash 
docker build -t kyc-verification-api .
```

### 🏃🏿‍♂️‍➡️ Run the container

```bash 
docker run -p 8080:80 \
-e "ConnectionStrings__DefaultConnection=your-connection-string"\
kyc-verification-api
```

## 📶 HTTP status codes

The API uses standard HTTP status codes:

- 200: Success
- 201: Created
- 400: Bad Request
- 401: Unauthorized
- 403: Forbidden
- 404: Not Found
- 500: Internal Server Error

## 📈 Rate Limiting

API requests are rate-limited except auth, hangfire dashboard and documentation endpoints by initially.
This is configurable in the app settings.
```json5
{
   "RateLimit" : {
      "PermitLimit" : 1, // Number of requests
      "Window" : 5, // Per number of seconds
      "QueueLimit" : 5, // Allowed requests in the queue
      // paths exempted from rate limiting
      "AllowedPaths" : [ 
         "auth",
         "scalar",
         "hangfire"
      ]
   }
}
```

## 📋 Logging
Logs are written to:
- Console
- File system (`/logs` directory)
- OpenTelemetry (if configured)
> To spin up an open telemetry injester quickly.
> ```bash
> docker run --rm -it -d \
>  -p 18888:18888 \
>  -p 4317:18889 \
>  --name aspire-dashboard \
>  mcr.microsoft.com/dotnet/aspire-dashboard:9.1
> ```
> then update the appsettings file accordingly
> ```json5
> {
>   "OtelConfing": {
>     "Endpoint": "http://localhost:4317", // default aspire url
>     "Enabled": false
>   }
> }
> ```
> [.NET Aspire dashboard overview
](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/dashboard/overview?tabs=bash)

## 🔄 Background Jobs with Hangfire

The API uses Hangfire for managing and executing background jobs.
In particular the calls made to the mocked external Id system.
This helps handle long-running tasks asynchronously, 
improving the API's responsiveness.

### Dashboard Access

The Hangfire dashboard is available at `/hangfire`.
To access the dashboard, navigate to:
> [https link](https://localhost:7174/hangfire) or 
> [http link](http://localhost:5160/hangfire)

It provides a real-time view of:
- Scheduled jobs
- Failed jobs
- Recurring jobs
- Job history
- Real-time job metrics

### Security

The Hangfire dashboard is open for this showcase. 
> In production, it should be both secured and require authentication.
> Only users with administrative access should view and manage jobs.

