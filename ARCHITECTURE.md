# EStore.API — Architecture

A .NET 10 e-commerce backend built with Clean Architecture and Domain-Driven Design.

## What it provides

| Area | Capabilities |
|------|--------------|
| Catalog | Products (CRUD, listing, details, view-count), hierarchical categories (recursive up/down traversal), product↔category assignment |
| Search | Product search with fuzzy matching and faceted filters (price, quantity, status, on-offer), sorting and pagination, with a SQL fallback |
| Cart | Get, add/remove item, clear, checkout → order |
| Orders | Place, list, by-customer, by-id, plus an approve / reject / update / cancel workflow |
| Offers | Percentage-discount and bundle-discount promotions, daily expiry job, offers linked to products and orders |
| Customers | Customer records |
| Identity | Register, login (JWT), roles, permission-based authorization, role/permission assignment |

## Layers

Dependencies point inward (`SharedKernel` ← `Domain` ← `Application` ← `Infrastructure`/`Presentation`).

| Project | Responsibility |
|---------|----------------|
| `SharedKernel` | Primitives: `Result` pattern, base entity/aggregate types, domain-event marker, and the in-house messaging contracts (`IRequest`, `IRequestHandler`, `INotification`, `IPipelineBehavior`, `ISender`, `IPublisher`). No external dependencies. |
| `Domain` | Entities, value objects, domain events, enums, business rules. |
| `Application` | Use cases (CQRS handlers), pipeline behaviors (validation, caching, idempotency, logging), domain-event handlers, infrastructure interfaces, and the mediator implementation. |
| `Contracts` | Public request/response DTOs. |
| `Infrastructure` | EF Core + Dapper persistence, migrations, repositories, JWT/auth, Elasticsearch, Quartz jobs, caching, idempotency, outbox. |
| `Presentation` | Controllers, Swagger, Mapster mapping, RFC-7807 error handling, DI composition root. |

## Technical topics

- **Clean Architecture** and **DDD** (aggregates, domain events, value objects)
- **CQRS** via an in-house mediator (replaces MediatR; see `Application/Common/Messaging`)
- **Outbox pattern** for reliable domain-event dispatch
- **Result pattern** for explicit, exception-free error handling
- **Pipeline behaviors** for cross-cutting concerns (logging → caching → idempotency → validation)
- **JWT authentication** with **permission-based authorization** (custom policy provider)
- **Dual data access**: EF Core for writes, Dapper for read-heavy/recursive queries
- **Elasticsearch** full-text product search with a database fallback
- **Cache-aside** caching, **idempotency** for selected endpoints, **FluentValidation**

## Runtime

`docker compose up -d --build` starts:

| Service | Purpose |
|---------|---------|
| `api` | The .NET 10 Web API (port 8080) |
| `postgres` | Primary datastore — EF Core + Dapper (port 5432) |
| `elasticsearch` | Product search index (port 9200) |

On startup the API applies EF Core migrations and seeds an admin user
(`admin@estore.com` / `estoreadmin`). Background jobs (Quartz): outbox dispatch
(every 60s), Elasticsearch re-sync (daily), offer-status management (daily), and
a product-cache warm-up (on start).

## Request flow

```
HTTP → Controller → ISender.Send(request)
                      → pipeline behaviors (logging → caching → idempotency → validation)
                        → IRequestHandler
                          → repository / EF Core / Dapper
                      ← Result<T>
                    ← ProblemDetails on failure
```

Domain events raised by aggregates are persisted to the outbox in the same
transaction, then published to their `INotificationHandler`s by the outbox job.

## Planned

- **Search**: replace Elasticsearch with PostgreSQL full-text search (`tsvector` +
  GIN, `pg_trgm` for typo tolerance), removing the Elasticsearch dependency.
  Optionally add `pgvector` for semantic search.
