---
name: EHR Identity Service setup
description: Runtime, SSL, and secrets requirements to get the Identity Service running on Replit.
---

## Runtime
- Project targets net8.0 — requires `dotnet-8.0` module (not dotnet-7.0).

## PostgreSQL SSL
- Replit managed postgres runs locally without SSL.
- The `BuildConnectionString` helper in `Program.cs` detects local vs cloud by checking if `PGHOST` contains a dot: local → `SSL Mode=Disable`, remote → `SSL Mode=Require;Trust Server Certificate=true`.

**Why:** Replit's local postgres socket does not support SSL negotiation; forcing SSL causes `NpgsqlException: SSL connection requested. No SSL enabled connection`.

## Required secrets
- `JWT_SECRET` — read from `Jwt:Secret` config or `JWT_SECRET` env var. Throws `InvalidOperationException` on startup if missing.
- `ENCRYPTION_KEY` — read from `Security:EncryptionKey` config or `ENCRYPTION_KEY` env var. Must be exactly 32 characters (AES-256). Throws on startup if missing.
- Set via `setEnvVars` for dev (non-secret env vars) or via `requestSecrets` for production.

## How to apply
Check these three things whenever the Identity Service fails to start:
1. `dotnet-8.0` module installed?
2. `JWT_SECRET` and `ENCRYPTION_KEY` set?
3. SSL mode correct for the postgres host?
