# Contributing to SylviaNG.Community (backend)

## Branching and pull requests

```
master  ->  dev  ->  community_engagement_<topic>   (your feature branch)
```

1. Branch from the latest `origin/dev`: `git fetch origin && git switch -c community_engagement_<topic> origin/dev`.
2. Commit in small, themed commits with clear messages.
3. Push the branch and open a pull request **into `dev`** (never directly into `master`).
4. Fill in the pull request template. `dev` is merged to `master` by the maintainers.

## Local setup

```bash
docker compose up -d postgres            # uses POSTGRES_* values from .env (copy .env.example to .env)
cd SylviaNG.Community
dotnet restore
dotnet ef database update                # applies all migrations
dotnet run                               # http://localhost:5210, Swagger at /swagger (Development only)
```

- Configuration keys are listed in `SylviaNG.Community/appsettings.Example.json`. Keep real
  secrets out of git (environment variables or `dotnet user-secrets`).
- Keycloak setup: see [KEYCLOAK_SETUP.md](KEYCLOAK_SETUP.md). Data model overview: [DATABASE_ARCHITECTURE.md](DATABASE_ARCHITECTURE.md).
- For local development, requests can use the dev-only headers `X-Dev-Employee-Id` and `X-Dev-Role`.

## Build and test

Run from the solution root:

```bash
dotnet build SylviaNG.Community.sln
cd SylviaNG.Community.Tests && dotnet test
dotnet test --filter "FullyQualifiedName~AnnouncementServiceTests"   # single class
```

If the API is running it locks `bin/`; build to a separate folder with `--artifacts-path <dir>`.

## Code conventions

New features are vertical slices under `Application/Features/<Feature>/{Commands,Queries,Models}`
following the `Announcements` feature: entity (inherits `Audit`) -> EF configuration ->
repository -> feature folder -> static mapper in `Application/Mappings` -> service interface and
implementation -> DI registration -> controller -> tests.

- No AutoMapper; use the static mapping extension classes.
- Validation lives in FluentValidation validators (run by the MediatR pipeline); handlers do not call validators.
- **Authorization:** every state-changing endpoint needs an explicit rule, either an `[Authorize(Policy = ...)]`
  attribute (`HRAdminOnly`, `HROnly`) or an ownership/role check in the service layer. The global filter only
  requires *some* authenticated user.
- Add or update tests (service, controller, validator) for every change.

## Database migrations

```bash
cd SylviaNG.Community
dotnet ef migrations add <Name>
dotnet ef database update
dotnet ef migrations script --no-build -o ../database_script.sql   # regenerate the consolidated script
```

Commit the migration, its `.Designer.cs`, the updated model snapshot, and the regenerated
`database_script.sql` together. `database_script.sql` is for **fresh, empty** PostgreSQL
databases only; existing databases are upgraded with `dotnet ef database update`.

## Before you open a pull request

- [ ] `dotnet build` and `dotnet test` pass
- [ ] Migrations and `database_script.sql` are committed if the model changed
- [ ] No secrets, personal data, or unrelated files (PDFs, local settings) are included
- [ ] `CHANGELOG.md` updated under **Unreleased**
