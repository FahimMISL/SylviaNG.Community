# Community Engagement System — Database Architecture

This document describes how the Community Engagement System's Postgres database is set up and how its schema came to be, plus a snapshot of the current employee data.

## 1. Overview

The `SylviaNG.Community` microservice persists its data in Postgres via EF Core. Locally it now runs against a **containerized Postgres instance** (Docker Compose) instead of the previously hardcoded shared remote host. The schema itself spans **10 functional modules** — Employee Directory, Teams, Profile Tagging, Notifications, Social Feed, Recognition, Survey & Feedback, Marketplace, Task Management, System/Admin logging, and Voting/Election — built out from two ERD diagrams that were already sitting in the repository.

## 2. Docker / Postgres setup

**Files added** (in `SylviaNG.Community-master/SylviaNG.Community-master/`):

- `docker-compose.yml` — a single `postgres:16` service, credentials sourced from `.env`, port `5432:5432`, data persisted in a named volume (`community_pgdata`).
- `.env.example` (committed) / `.env` (gitignored) — `POSTGRES_DB`, `POSTGRES_USER`, `POSTGRES_PASSWORD`.
- `SylviaNG.Community/appsettings.json` — `Database:ConnectionString` changed from the old shared host (`192.168.1.212`) to `Host=localhost;Port=5432;...`. No code changes were needed: `Database:Provider=Postgresql` and the provider-selection logic in `Infrastructure/Extensions/DependencyInjection.cs` already read from config.

**To bring it up:**

```bash
cd SylviaNG.Community-master/SylviaNG.Community-master
docker compose up -d
cd SylviaNG.Community
dotnet ef database update
dotnet run
```

## 3. How the schema was derived and built

### 3.1 Recovering the ERD source

Two draw.io exports already existed in the repo: `SylviaNG.Community/erd.drawio.png` (a single compact diagram of all entity relationships) and `erd_broken.drawio.png` (the same model split into 10 per-module diagrams with full field lists). Both PNGs were too compressed to read reliably by eye, but draw.io embeds its diagram source as a `tEXt` chunk inside the PNG file itself. That chunk was extracted and decoded (base64 → raw-deflate decompression → URL-decode) to recover the exact mermaid ER-diagram source — giving authoritative entity names, field types, and relationships instead of a visual guess.

### 3.2 The vertical-slice pattern

Every new entity follows the same 8-step pattern already established by the `Announcements` and `Employees` features:

1. **Domain** — entity class in `Domain/Entities/`, inheriting `SharedKernel.Audit.Audit` (provides `long`-based `CreatedAt/CreatedBy/UpdatedAt/UpdatedBy/DeletedAt/DeletedBy/Status/TenantId/Remarks`).
2. **Infrastructure** — `IEntityTypeConfiguration<T>` in `Infrastructure/Configurations/` (auto-discovered via `modelBuilder.ApplyConfigurationsFromAssembly`), repository in `Infrastructure/Repositories/` extending the generic `Repository<T>`.
3. **Application** — feature folder in `Application/Features/{Module}/` with `Models/`, `Commands/`, `Queries/` (MediatR command/query + handler + FluentValidation validator).
4. **Mapping** — static extension class in `Application/Mappings/`.
5. **Services** — interface in `Application/Interfaces/Services/`, implementation in `Application/Services/`.
6. **DI** — repository registered in `Infrastructure/Extensions/DependencyInjection.cs`, service in `Application/Extensions/DependencyInjection.cs`.
7. **Controller** — MediatR-based, `[Authorize(Policy = "HRAdminOnly")]` on admin/moderation actions.
8. **Tests** — service, controller, and validator tests in `SylviaNG.Community.Tests/`.

Pure log tables (`AuditLog`, `ActivityLog`, `TaskHistory`) deviate slightly: they only get a paged **query** endpoint, since rows are inserted internally by the action that generates them, not through a public "create log entry" endpoint.

### 3.3 Reconciling the ERD against the real codebase

The ERD's design didn't perfectly match what already existed in code, so a few decisions were made deliberately:

| ERD said | Decision |
|---|---|
| `Department`/`Branch`/`Designation`/`Role`/`Permission` as local tables | **Not built.** These remain resolved live via `ICoreGrpcClient` (gRPC to the Core microservice) — no local copies, matching the existing architecture. |
| A separate normalized `EmployeeProfile` table, plus `Skill`/`Interest` junction tables | **Additive only.** `Employee`'s existing flat `Bio`/`Extension`/visibility columns and flat `Skills`/`Interests` strings were left untouched; only new tagging tables (`Skill`, `EmployeeSkill`, `Interest`, `EmployeeInterest`, `Badge`, `EmployeeBadge`) were added. |
| Voting/Election module modeled with generic `Users`/`Teams` entities and `UUID` keys | **Adapted** to reference the real `Employee`/`Team` entities with `long` keys, consistent with the rest of the system. |

### 3.4 Build execution

The `Team`/`TeamMember` module was built first, by hand, as the concrete reference implementation. The remaining 9 modules were then built by **7 parallel background agents**, each scoped to a non-overlapping set of new files (no agent touched another's entities, and none touched the 3 shared registration files directly, to avoid conflicts in a non-git working directory). Each agent reported back the exact `DbSet`/DI lines its module needed.

One cross-cutting issue surfaced during this process: the Task Management module's `Task` entity collides with `System.Threading.Tasks.Task` (implicitly imported everywhere in .NET). It was resolved by qualifying ambiguous references (`Domain.Entities.Task` / fully-qualified `System.Threading.Tasks.Task`) wherever both appear in the same file, rather than renaming the entity.

### 3.5 Central wiring and migration

Once all modules were built, the reported `DbSet` declarations and repository/service registrations were applied in one pass to the three shared files (`Infrastructure/Data/ApplicationDBContext.cs`, `Infrastructure/Extensions/DependencyInjection.cs`, `Application/Extensions/DependencyInjection.cs`). A small consistency fix was also applied: five new entities with a string `Status` field (`ContentReport`, `Conversation`, `Election`, `Listing`, `MarketplaceReport`) needed the `new` keyword to intentionally shadow `Audit.Status`, matching the existing `Announcement.Status` convention.

A single consolidated EF Core migration, `AddCommunityEngagementModules`, was then generated and applied — this was the practical choice over "one migration per module" because EF Core's `ApplyConfigurationsFromAssembly` pulls every entity configuration already compiled into the assembly into the model regardless of migration staging, so the model diff was already "everything at once" by the time all agents had finished.

**Verification performed:**
- `dotnet build` — 0 errors.
- `dotnet test` — 290/290 passing.
- `dotnet run` against the local container, then `GET /community/employee/paged` (returns the 3 seeded employees) and `GET /community/team/paged` (returns an empty, correctly-paginated list) confirmed end-to-end.

## 4. Module / table summary

| # | Module | Tables |
|---|---|---|
| — | Core (pre-existing) | `Announcements`, `Employees` |
| 1 | Team | `Teams`, `TeamMembers` |
| 2 | Profile tagging | `Skills`, `EmployeeSkills`, `Interests`, `EmployeeInterests`, `Badges`, `EmployeeBadges` |
| 3 | Notifications | `Notifications`, `NotificationPreferences` |
| 4 | Social Feed | `Posts`, `PostAttachments`, `PostComments`, `PostReactions`, `CommentReactions`, `Mentions`, `Polls`, `PollOptions`, `PollVotes`, `ContentReports` |
| 5 | Recognition | `Recognitions`, `RecognitionReactions`, `RecognitionComments` |
| 6 | Survey & Feedback | `Surveys`, `SurveyAudiences`, `SurveyQuestions`, `SurveyOptions`, `SurveyResponses`, `SurveyAnswers` |
| 7 | Marketplace | `Listings`, `ListingImages`, `Favorites`, `Conversations`, `ConversationParticipants`, `Messages`, `MarketplaceReports` |
| 8 | Task Management | `RecurringTasks`, `Tasks`, `TaskComments`, `TaskAttachments`, `TaskHistories` |
| 9 | System/Admin | `DashboardPreferences`, `ActivityLogs`, `AuditLogs`, `FileStorages` |
| 10 | Voting/Election | `Elections`, `ElectionAudienceTargets`, `ElectionCandidates`, `ElectionVotes` |

**53 application tables** in total (plus EF Core's own `__EFMigrationsHistory`).

## 5. Employee list

Current contents of the `Employees` table (seeded dev personas):

| EmployeeId | Code | Name | Email | Phone | Ext. | Division | DepartmentId | DesignationId | SiteId | Active |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | EMP00001 | Ayesha Rahman | ayesha.rahman@sylviang.example | +880-1710-000001 | 1001 | Engineering | 1 | 1 | 1 | ✅ |
| 2 | EMP00002 | Tanvir Hasan | tanvir.hasan@sylviang.example | +880-1710-000002 | 1002 | Engineering | 1 | 2 | 1 | ✅ |
| 3 | EMP00003 | Farhana Akter | farhana.akter@sylviang.example | +880-1710-000003 | 1003 | People & Culture | 2 | 3 | 1 | ✅ |

`DepartmentId`/`DesignationId`/`SiteId` are raw IDs — their display names are resolved at read time via `ICoreGrpcClient` from the separate Core microservice, not stored locally.

## 6. Reproducing / verifying locally

```bash
# from SylviaNG.Community-master/SylviaNG.Community-master/
docker compose up -d                 # start Postgres
cd SylviaNG.Community
dotnet ef database update            # apply all migrations
dotnet build ../SylviaNG.Community.sln
dotnet test ../SylviaNG.Community.Tests
dotnet run                           # http://localhost:5210, Swagger at /swagger
```

Quick smoke test (dev-only header auth):

```bash
curl -H "X-Dev-Employee-Id: 1" -H "X-Dev-Role: HRAdmin" \
  "http://localhost:5210/community/employee/paged?page=1&pageSize=5"

curl -H "X-Dev-Employee-Id: 1" -H "X-Dev-Role: HRAdmin" \
  "http://localhost:5210/community/team/paged?page=1&pageSize=5"
```
