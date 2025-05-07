# KYC Verification API

A simple yet functional KYC (Know Your Customer) verification REST API built using **.NET 9**. 
This API simulates verifying a customer's details.

## 🚀 Features

- ✅ Verify customer identity using a mock external service
- 📡 RESTful API with Scalar UI documentation
- ✅ Unit tests using xUnit and Moq
- ✅ Background jobs to handle long taks
- 🐳 Optional Docker setup

## 🛠️ Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Postgres Database](https://www.docker.com/blog/how-to-use-the-postgres-docker-official-image/)
To quickly set it up locally:
`docker run --name some-postgres -e POSTGRES_PASSWORD=mysecretpassword -d postgres
`

### Run Locally

```bash
git clone https://github.com/edcsu/KYCVerificationAPI.git

cd KycVerificationApi

dotnet run
```

## 📁 Project Structure

```
/KYCVerificationAPI
├── KYCVerificationAPI/
├── KYCVerificationAPI.Tests/
└── README.md
```

## 🔒 Auth

This API is protected using JWT token.
1. Make a request to the token endpoint.
There should be atleast one custom claim for `client:true` to use
the verification endpoints.

`Token endpoint: https://localhost:7174/api/auth/token`
The request body
```json
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

## 🔒 Disclaimer
This is a demo project.
The mock external service and
does not represent a real ID verification provider.