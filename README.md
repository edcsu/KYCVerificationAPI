# KYC Verification API

A simple yet functional KYC (Know Your Customer) verification REST API that is built using **.NET 9**.
It is designed to simulate the process of validating a user's identity against a national ID system. 
The project highlights my ability to solve real-world problems through [vertical slice architecture](https://blog.ndepend.com/vertical-slice-architecture-in-asp-net-core/).


## 🚀 Why This Feature

In many financial and identity-sensitive applications, 
verifying a user’s national ID is a critical step. 
This API mimics such a scenario, integrating:

- ✅ Clean separation of concerns with vertical slice architeture
- ✅ Secured REST API using JWT tokens
- ✅ Verify customer identity using a mock external service
- 📡 RESTful API fully documented with [Scalar UI](https://guides.scalar.com/scalar/scalar-api-references/net-integration)
- ✅ Unit tests using xUnit and Moq
- ✅ Integration tests using xUnit and test containers
- ✅ Background jobs to handle long-running tasks
- ✅ Error handling and retry logic
- ✅ Robust input validation
- ✅ OpenTelemetry ready
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
        }
      }
    ``` 
   
4. Run the application
    ```bash 
      dotnet run
    ````
> The API will be available at `https://localhost:7174` 
> or `http://localhost:5160`
> By default, it opens in the API documentation link.

## API Endpoints

> The API [https documentation can be accessed her](https://localhost:7174) while
> the [http documentation is here](http://localhost:5160)

A summary of the endpoints is shown below

### 🔎 Verifications

| Method | Endpoint                  | Description | Auth Required  |
|--------|---------------------------|-------------|----------------|
| POST   | `/api/verifications`      | Create new verification | Yes            |
| GET    | `/api/verifications/{id}` | Get verification by ID | Yes            |
| POST   | `/api/auth/token`         | Delete verification | No             |

### 📝 Sample Requests

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


#### 🔒 Get Access token

The API uses JWT Bearer authentication.
Include the token in the Authorization header.

```http
Authorization: Bearer your-token-here
```

For this showcase, the token is generated as shown below:

1. Make a request to the token endpoint.
There should be at least one custom claim for `client:true` to use
the verification endpoints. This mimics how I setup clients to use the solution.

`Token endpoint: https://localhost:7174/api/auth/token`
The request body
```json lines
{
  "userId": "2032f3c8-ecc3-4205-94d1-5b05d2ea7c65",
  "email": "m@example",
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
      "email": "m@example",
      "customClaims": {
        "client": true
      }
    }'
 ```

## ⚠️ Disclaimer
This is a demo project.
The mock external service does not represent a real ID verification provider.
It shows the interaction that can be there.

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

- Execute the test suite using:
```bash 
dotnet test
```

- For specific test projects:
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

API requests are rate-limited outside the verification, auth, and documentation endpoints.

## 📋 Logging
Logs are written to:
- Console
- File system (`/logs` directory)
- OpenTelemetry (if configured)
> to spin up quickly one
> ```bash
> docker run --rm -it -d \
>  -p 18888:18888 \
>  -p 4317:18889 \
>  --name aspire-dashboard \
>  mcr.microsoft.com/dotnet/aspire-dashboard:9.1
> ```
> [.NET Aspire dashboard overview
](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/dashboard/overview?tabs=bash)
