# Special Unit App

Local full-stack workspace for the Special Unit management app.

This project was made as a graduation project for a Bachelor's degree from ISTIC University. It was developed with my colleague Mohamed Mostfa Jadoui: Mohamed built the frontend, and I built the backend.

## Structure

```text
sp_backend/
  backend/   .NET API
  frontend/  Angular app
```

## Requirements

- .NET 8 SDK
- Node.js and npm
- Docker Desktop with SQL Server container available locally

## Environment

Local settings live in `.env` at the workspace root. The file is ignored by git.

To recreate it on another machine:

```powershell
Copy-Item .env.example .env
```

Then edit `.env` with the local SQL Server password and JWT secret.

## Run Locally

Start the database container:

```powershell
docker start sp_sql
```

Start the backend:

```powershell
cd backend
dotnet run
```

Start the frontend in another terminal:

```powershell
cd frontend
npm ci
npm start
```

URLs:

```text
Frontend: http://localhost:4200
Backend:  http://localhost:5038
Swagger:  http://localhost:5038/swagger
```

Default seeded superadmin:

```text
Username: superadmin
Password: SuperPass123!
```

## Notes

- Keep `.env` local. Use `.env.example` for shared placeholders.
- Frontend API services currently expect the backend at `http://localhost:5038`.
- Backend seeds the superadmin account when no `SuperAdmin` account exists.
