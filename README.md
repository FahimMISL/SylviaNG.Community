# SylviaNG Community Microservice

## Overview

The Community microservice is part of the SylviaNG HRMS ecosystem, responsible for
managing organization-wide announcements published to employees (company news, events,
policy updates, holidays, etc.). It ships one aggregate root — `Announcement`.

## Technology Stack

- .NET 10.0
- Entity Framework Core 10.0
- PostgreSQL / SQL Server / Oracle (configurable via `Database:Provider`)
- Keycloak Authentication (JWT)
- Apache Kafka for event-driven architecture (employee sync)
- Finbuckle.MultiTenant for multi-tenancy support
- MediatR for CQRS pattern
- FluentValidation for input validation
- **Manual object mapping** (static mapper extensions — no AutoMapper)
- gRPC for inter-service communication

## Project Structure

```
SylviaNG.Community/
├── Application/                        # Application layer (business logic, CQRS handlers)
│   ├── Common/
│   │   ├── Exceptions/                # Custom exceptions (NotFoundException, DuplicateException)
│   │   └── Models/                    # Shared DTOs (CoreGrpcModels)
│   ├── Extensions/
│   │   ├── AuthenticationExtensions.cs  # Keycloak JWT authentication setup
│   │   ├── AuthorizationExtensions.cs   # Authorization policy configuration
│   │   ├── DependencyInjection.cs       # Application service registrations
│   │   └── ValidationBehavior.cs        # MediatR pipeline validation behavior
│   ├── Features/
│   │   └── Announcements/             # Feature module (follow this pattern for new features)
│   │       ├── Commands/              # CQRS Commands (Create, Update, Delete + Handlers + Validators)
│   │       ├── Models/                # DTOs (Request/Response models)
│   │       └── Queries/               # CQRS Queries (GetAll, GetById, GetPaged + Handlers)
│   ├── Interfaces/
│   │   ├── Externals/                 # External service interfaces (ICoreGrpcClient)
│   │   ├── Repositories/              # Repository interfaces (IAnnouncementRepository)
│   │   └── Services/                  # Service interfaces (IAnnouncementService)
│   ├── Mappings/                      # Manual mapper extension classes (AnnouncementMapper)
│   └── Services/                      # Business logic service implementations
├── Domain/                            # Domain layer (entities, value objects, domain events)
│   ├── Entities/                      # Business entities (Announcement, Employee)
│   ├── Enums/                         # Domain enumerations
│   └── Events/                        # Domain events
├── Infrastructure/                    # Infrastructure layer (data access, external services)
│   ├── Configurations/                # EF Core entity configurations
│   ├── Data/
│   │   └── ApplicationDBContext.cs   # DbContext with multi-tenancy
│   ├── Extensions/
│   │   ├── DependencyInjection.cs     # Infrastructure service registrations
│   │   └── GrpcExtensions.cs          # gRPC client registration
│   ├── Interceptors/                  # EF Core interceptors (UtcDateTime)
│   ├── Kafka/                         # Kafka consumers (EmployeeEventConsumer)
│   ├── MultiTenancy/                  # Tenant info model
│   ├── Repositories/                  # Repository implementations
│   └── Services/                      # External service implementations (CoreGrpcClient)
├── Controllers/                       # API controllers
│   └── AnnouncementController.cs      # Announcement endpoints
├── Middlewares/                       # Custom middleware components
│   ├── GlobalExceptionHandlerMiddleware.cs
│   └── ResponseWrappingMiddleware.cs
├── SharedKernel/                      # Shared components
│   ├── Audit/                         # Base audit entity
│   ├── Generic/                       # Generic repository + unit of work
│   ├── Pagination/                    # Pagination support
│   └── Utils/                         # DateTime utilities, JSON converters
├── Protos/                            # gRPC proto definitions
│   └── core.proto
├── Migrations/                        # EF Core migrations
├── Program.cs                         # Application entry point
├── appsettings.json                   # Configuration
└── Dockerfile                         # Docker support

SylviaNG.Community.Tests/              # Unit and integration tests
├── Controllers/                       # Controller tests
├── Services/                          # Service tests
└── Validators/                        # Validator tests
```

## Architecture Pattern

This project follows **Clean Architecture** with **Domain-Driven Design (DDD)** and **CQRS**:

```
┌──────────────────────────────────────────────────┐
│                   Controllers                     │  ← API endpoints
├──────────────────────────────────────────────────┤
│                  Application                      │  ← Business logic, CQRS, Services
│           (MediatR Handlers, Validators)          │
├──────────────────────────────────────────────────┤
│                    Domain                         │  ← Entities, Events, Enums
├──────────────────────────────────────────────────┤
│                Infrastructure                     │  ← Data access, Kafka, gRPC
│         (EF Core, Repositories, Interceptors)     │
├──────────────────────────────────────────────────┤
│                SharedKernel                       │  ← Generic repo, Audit, Pagination
└──────────────────────────────────────────────────┘
```

## Getting Started

### Prerequisites

- .NET 10.0 SDK
- PostgreSQL / SQL Server / Oracle database
- Keycloak instance for authentication
- Apache Kafka (for employee sync events)

### Configuration

Update `appsettings.json` with your database and Keycloak settings:

```json
{
  "Database": {
    "Provider": "Postgresql",
    "ConnectionString": "Host=localhost;Port=5432;Database=community_db;Username=user;Password=pass"
  },
  "Keycloak": {
    "Authority": "http://localhost:8082/realms/sylviang",
    "ClientId": "sylviang-api",
    "ClientSecret": "your-client-secret"
  }
}
```

### Running the Service

```bash
cd SylviaNG.Community
dotnet restore
dotnet run
```

The service will start on:

- HTTP: http://localhost:5210
- HTTPS: https://localhost:7210

### API Documentation

Once running, access Swagger UI at: `http://localhost:5210/swagger`

## Features

- **Multi-tenant support** via JWT claims (`tenant_id`)
- **Clean Architecture** with strict layer separation
- **CQRS with MediatR** — Commands and Queries separated
- **Repository Pattern** with generic base + Unit of Work
- **Global exception handling** with consistent error response format
- **Response wrapping middleware** — All responses wrapped in `{ hasError, decentMessage, errorDetails, content }`
- **Audit logging** — All entities inherit from `Audit` base class
- **UTC DateTime enforcement** via EF Core interceptor
- **Manual object mapping** — static mapper extensions per feature (no AutoMapper)
- **Event-driven architecture** with Kafka (employee sync)
- **gRPC** for inter-service communication with Core microservice
- **FluentValidation** integrated into MediatR pipeline

## API Response Format

All API responses follow this standard envelope:

```json
{
  "hasError": false,
  "decentMessage": "Request processed successfully.",
  "errorDetails": null,
  "content": { }
}
```

Error responses:

```json
{
  "hasError": true,
  "decentMessage": "Validation failed.",
  "errorDetails": ["Title is required."],
  "content": null
}
```

## Database Migrations

```bash
# Add a new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update
```

> Note: the template's `Employee` entity carries model-only navigation properties (no
> migration yet), so the first `migrations add` bundles some extra `AddForeignKey`
> operations — that is expected, not a bug.

## Testing

```bash
cd SylviaNG.Community.Tests
dotnet test
```

## Docker Support

Build and run using Docker:

```bash
docker build -t sylviang-community .
docker run -p 5210:5210 sylviang-community
```

## How to Add a New Feature

Follow the existing `Announcements` pattern:

1. **Domain** — Create entity in `Domain/Entities/` inheriting from `Audit`
2. **Infrastructure** — Add `DbSet` in `ApplicationDBContext`, create configuration in `Configurations/`, create repository in `Repositories/`
3. **Application** — Create feature folder in `Features/` with `Commands/`, `Queries/`, `Models/` subfolders
4. **Mappings** — Add a **manual** mapper class (static extension methods) in `Mappings/` — `ToEntity`, `ApplyUpdate`, `ToResponse`, `ToLookupResponse`
5. **Services** — Create service interface in `Interfaces/Services/` and implementation in `Services/`
6. **DI** — Register repository in `Infrastructure/Extensions/DependencyInjection.cs` and service in `Application/Extensions/DependencyInjection.cs`
7. **Controller** — Create controller in `Controllers/` using MediatR for CQRS
8. **Tests** — Add service, controller, and validator tests in `Tests/`

## Related Projects

- **SylviaNG.Recruitment** — Recruitment management microservice (canonical copy-source)
- **SylviaNG.LMS** — Learning Management System microservice
- **SylviaNG.Cafeteria** — Cafeteria management microservice