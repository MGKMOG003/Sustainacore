# Sustainacore.Domain

Core domain layer — entities, value objects, domain events, and core interfaces. No external dependencies.

## Contents
- `/Entities` – Aggregate roots & entities (e.g., Project, MaintenanceTicket, Contractor, Quote, Invoice)
- `/ValueObjects` – Email, Money, Address, FileRef
- `/Events` – Domain events (e.g., QuoteAccepted, InvoicePaid, TicketAssigned)
- `/Abstractions` – Interfaces shared upward (e.g., IHasDomainEvent)

## Guidelines
- Keep it persistence-agnostic and framework-agnostic.
- Business rules live here.
- No direct EF Core types.