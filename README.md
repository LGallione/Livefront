# Referral System Mock API

This repository contains a mock API service for the referral system feature. The service is built using .NET Core and provides endpoints for managing referrals, generating referral links, and tracking referral statistics.

## Prerequisites

- [.NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0) or later
- Any modern IDE (Visual Studio, VS Code, Rider, etc.)

## Building the Solution

```bash
dotnet build
```

## Running Tests

```bash
dotnet test
```

## Running the API

```bash
cd ReferralSystem.Api
dotnet run
```

The API will be available at `http://localhost:5000` and `https://localhost:5001`.

## API Documentation

The API documentation is available via Swagger UI at `/swagger` when running the application.

### Available Endpoints

- `POST /api/v1/referrals/links` - Generate a new referral link
- `GET /api/v1/referrals/stats` - Get referral statistics
- `POST /api/v1/referrals/validate` - Validate a referral link

## Development

The solution consists of two projects:
- `ReferralSystem.Api` - The main API project
- `ReferralSystem.Tests` - Unit and integration tests

## Cross-Platform Compatibility

This solution is fully compatible with macOS, Windows, and Linux environments. 