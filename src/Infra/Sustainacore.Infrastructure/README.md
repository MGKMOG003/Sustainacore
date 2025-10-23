# Sustainacore.Infrastructure

Infrastructure layer — EF Core, external services, and implementations.

## Contents
- `/Persistence` – DbContext, configurations, migrations
- `/Email`, `/Sms`, `/Storage` – Provider adapters
- `/Payments` – Gateway abstractions (e.g., PayFast) and concrete impls
- `/Identity` – ASP.NET Identity stores and configuration

## Dev Setup
- Configure connection string in `appsettings.Development.json` or env var `ConnectionStrings__Default`.
- Apply migrations from this project with the API as the startup project.
