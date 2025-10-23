# Sustainacore.Application

Application layer — use cases, DTOs, validators, and orchestration.

## Contents
- `/Features/*` – Vertical slices (commands/queries with MediatR)
- `/DTOs` – Data transfer types
- `/Behaviors` – Pipeline behaviors (validation, logging, perf)
- `/Interfaces` – Contracts to be implemented in Infrastructure

## Notes
- No EF Core here (only abstractions).
- Use FluentValidation for request validation.
- Raise domain events via entities and publish through handlers.
