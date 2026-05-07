# Special Unit Backend

.NET 8 API for the Special Unit management app.

## Location

```text
sp_backend/
  backend/
  frontend/
```

This backend reads local configuration from the root `.env` file.

## Required Settings

The backend expects these values from `../.env` or environment variables:

```text
BACKEND_URLS
ConnectionStrings__spApiDb
ConnectionStrings__BackupDbContext
Jwt__Key
Jwt__Issuer
Jwt__Audience
```

Use `../.env.example` as the template.

## Run

From this folder:

```powershell
dotnet restore
dotnet run
```

The API listens on:

```text
http://localhost:5038
```

Swagger is available in development:

```text
http://localhost:5038/swagger
```

## Database

The local setup uses SQL Server on `localhost:1433`. Start the local container before running the API:

```powershell
docker start sp_sql
```

The app calls `EnsureCreated()` at startup and seeds a `SuperAdmin` account if one does not already exist.

## Project Map

```text
Contollers/   API controllers
Data/         Entity Framework DbContext
DTO/          Request and response DTOs
Interfaces/   Service contracts
Mappers/      AutoMapper profiles and mappers
Migrations/   EF migrations
Models/       Entity models
Services/     Business logic and hosted background services
```

## Useful Commands

```powershell
dotnet build
dotnet run
```
