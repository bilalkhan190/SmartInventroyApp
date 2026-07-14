# SmartInventory API

SmartInventory is a .NET 8 REST API for inventory and procurement workflows. It covers products, categories, suppliers, stock, purchase orders, approvals, and goods receiving, with JWT authentication and rotating refresh tokens.

## Architecture

- `SmartInventory.Domain` — entities, domain rules, events, and enums
- `SmartInventory.Application` — use cases, validation, contracts, and MediatR handlers
- `SmartInventory.Infrastructure` — EF Core, SQL Server, persistence, and seeding
- `SmartInventory.Presentation` — Minimal API endpoints, middleware, and JWT authentication

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server LocalDB or SQL Server
- EF Core CLI: `dotnet tool install --global dotnet-ef`

## Local setup

1. Create your local settings file:

   ```powershell
   Copy-Item SmartInventory.Presentation/appsettings.example.json SmartInventory.Presentation/appsettings.Development.json
   ```

2. Update the connection string, generate a random JWT key of at least 32 bytes, and choose a development admin password. Never commit the resulting settings file.

3. Apply migrations and start the API:

   ```powershell
   dotnet restore
   dotnet ef database update --project SmartInventory.Infrastructure --startup-project SmartInventory.Presentation
   dotnet run --project SmartInventory.Presentation
   ```

The default launch profile listens on `http://localhost:56208` and `https://localhost:56207`. Swagger is available in Development at `/swagger`.

## Authentication

| Method | Endpoint | Purpose |
| --- | --- | --- |
| `POST` | `/api/auth/register` | Create an account |
| `POST` | `/api/auth/login` | Sign in and receive access/refresh tokens |
| `POST` | `/api/auth/refresh` | Rotate the refresh token |
| `POST` | `/api/auth/logout` | Revoke the refresh token |

All inventory and procurement endpoints require `Authorization: Bearer <access-token>`.

Refresh tokens are random, stored only as SHA-256 hashes, rotated on use, and revocable. Passwords are stored using PBKDF2-SHA256 with a unique random salt.

## Configuration

Production secrets should be supplied through a secret manager or environment variables:

```text
ConnectionStrings__DefaultConnection
Jwt__Issuer
Jwt__Audience
Jwt__Key
Jwt__AccessTokenMinutes
Jwt__RefreshTokenDays
```

Do not configure `SeedAdmin` in production. Create production accounts through an approved provisioning flow.

## Build

```powershell
dotnet build SmartInventoryApp.slnx --configuration Release
```

## Frontend

The React client is maintained separately in the `smartinventory-ui` project. Set its `VITE_API_URL` to this API's `/api` URL.

## Security

See [SECURITY.md](SECURITY.md) for vulnerability reporting and secret-handling guidance.
