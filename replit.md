# Modern EHR Platform

A production-ready, enterprise-grade Electronic Health Records system built with ASP.NET Core microservices and Angular 18.

## Stack

- **Backend**: ASP.NET Core (.NET 8), EF Core, PostgreSQL, MediatR (CQRS), FluentValidation, Serilog, Mapster
- **Frontend**: Angular 18, Tailwind CSS (separate `frontend/` directory — not yet wired to a workflow)
- **Database**: Replit managed PostgreSQL (auto-provisioned)

## Project Structure

```
backend/
  src/
    EHRPlatform.Common/          # Shared: CQRS, repository, UoW, domain events, security
    EHRPlatform.Services.Identity/  # Auth microservice (running)
    EHRPlatform.Services.Patient/   # Patient microservice (foundation only)
    EHRPlatform.Services.Clinical/  # Clinical microservice (foundation only)
    ... (8 more microservices — foundation scaffolded)
frontend/                        # Angular 18 app (complete UI, not yet running on Replit)
docs/                            # Architecture, API spec, security, DB schema
```

## Running the Identity Service

The workflow **Identity Service** runs the backend auth service:

```
cd backend && dotnet run --project src/EHRPlatform.Services.Identity
```

- Starts on **port 5000**
- Swagger UI: `http://localhost:5000/swagger`
- Health check: `http://localhost:5000/health`
- On first start: automatically creates all DB tables and seeds default roles

## Required Environment Variables

Set in Replit Secrets / environment:

| Variable         | Purpose                                       |
|------------------|-----------------------------------------------|
| `JWT_SECRET`     | Signs JWT access tokens (64-char hex)         |
| `ENCRYPTION_KEY` | AES-256 key for PHI encryption (32 chars)     |
| `PGHOST` etc.    | Auto-provided by Replit managed PostgreSQL    |

## Database Strategy (from spec)

Polyglot persistence target:
- **PostgreSQL** — primary OLTP (EF Core + Dapper)
- **Redis** — caching, sessions, rate limiting
- **Elasticsearch** — full-text patient/record search
- **MongoDB** — clinical documents, audit logs (high volume)

Only PostgreSQL is active for the Identity Service. The `EHRPlatform.Common` library already has Redis and Elasticsearch clients wired up for future services.

## Identity Service — Endpoints

| Method | Path                         | Auth     | Description                  |
|--------|------------------------------|----------|------------------------------|
| POST   | /api/v1/auth/register        | None     | Self-register a new user     |
| POST   | /api/v1/auth/login           | None     | Login → JWT + refresh token  |
| POST   | /api/v1/auth/refresh         | None     | Refresh access token         |
| POST   | /api/v1/auth/logout          | Bearer   | Revoke refresh token         |

## What Was Done During Setup

- Upgraded runtime module from dotnet-7.0 → dotnet-8.0
- Generated JWT_SECRET and ENCRYPTION_KEY automatically
- Fixed all compile errors (duplicate class definitions across validator/command files)
- Removed 10 junk/duplicate files from the Application layer
- Extracted 4 inline domain event classes into proper `Domain/Events/` files
- Added `appsettings.Development.json`
- Fixed PostgreSQL SSL mode for Replit local database

## User Preferences

- Deep, thorough refactoring preferred — clean up duplicates and legacy patterns
- Backend-first focus; Identity Service is the starting point
