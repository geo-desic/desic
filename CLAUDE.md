# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

Desic is a .NET solution **template** — a reference starting point for new APIs following DDD / Clean Architecture. The included entity types are deliberately minimal; the value is the boilerplate, infrastructure, and testing framework. Targets **.NET 10**. Solution file is `Desic.slnx` (the new XML-based format).

## Common commands

All commands run from the top-level solution directory (PowerShell).

```powershell
# Build
dotnet build Desic.slnx

# Test (Microsoft.Testing.Platform runner, configured in global.json)
dotnet test
dotnet test --ignore-exit-code 8 -- --filter-trait "Type=Unit"          # Unit only
dotnet test --ignore-exit-code 8 -- --filter-trait "Type=Integration"    # Integration only
dotnet test --ignore-exit-code 8 -- --filter-trait "Type=Functional"     # Functional only
# --ignore-exit-code 8 prevents failures when a project has 0 matching tests for the filter

# Tests with coverage report (writes coverage-report/index.html)
./tools/dotnet-test-with-coverage-report.ps1

# Code format verification (CI runs this; it fails the build on non-compliant code)
./tools/dotnet-format-verification.ps1   # = dotnet format Desic.slnx --verify-no-changes

# Frontend (src/web)
npm run dev      # vite dev server
npm run build    # tsc -b && vite build
npm run lint     # eslint
```

Running a single test: use `--filter` with the test runner, e.g.
`dotnet test test/Application.Tests.Unit -- --filter "FullyQualifiedName~CreateUser"`.

## Running the application

**Preferred:** launch the **AppHost** project (Aspire) with the `https` profile. AppHost orchestrates the full dependency graph in order: database resource → `db-updater` (runs to completion) → `api` → `web` (Vite). It can auto-create the required user secrets.

Individual projects (`Api`, `Infrastructure.Tools.DbUpdater`) can be launched directly, but only if their dependencies (a running DB server, an initialized/migrated/seeded application database) are already available.

The active database is chosen at runtime by the `DbProvider` configuration value: `Sqlite` or `SqlServer` (default in the Api). AppHost provisions the matching resource and passes the connection string down to `db-updater` (via `--ConnectionStrings:*` args) and `api` (via `ConnectionStrings__*` env vars).

## Architecture

Clean Architecture with strict layer dependencies (Domain ← Application ← Infrastructure ← Api). Each layer's services are registered through an `AddDomain()` / `AddApplication()` / `AddInfrastructure()` extension (`DependencyInjection.cs` per project), wired together in `Api/Program.cs`.

Projects under `src/`:
- **Domain** — entities and domain rules. Entities derive from `BaseEntity` → `CreatableEntity` → `ModifiableEntity` → `SoftDeletableEntity`. `SystemEntityTypes.cs` is the central registry of entity types.
- **Application** — use cases, organized by **vertical slice** (e.g. `Users/Create/`, `Iso3166Countries/List/`). Each slice typically has a `*Request`, `*RequestHandler`, `*Result`, `*Validator`, and a model class. Defines `IApplicationDbContext` and the pagination/ordering/filtering infrastructure in `Common/`.
- **Infrastructure** — `ApplicationDbContext`, EF Core entity configurations (`Data/Configurations/`), seeding. Provider-agnostic.
- **Infrastructure.Data.Providers / .Sqlite / .SqlServer** — provider-specific concerns. Each provider library holds its own EF Core migrations. To add a new database provider, create a parallel `Infrastructure.Data.<Provider>` library.
- **Shared** — cross-cutting helpers, including the `Mediator/` pipeline behaviors (e.g. `LoggingBehavior`) and `GuidExtensions` (sequential GUID generation).
- **Api** — ASP.NET Core controllers (`Controllers/V1/`), DTOs, health checks, startup background service. Controllers are thin: they build a request and call `_mediator.Send(...)`.
- **ServiceDefaults** — shared Aspire service configuration (telemetry, health, resilience).
- **AppHost** — Aspire orchestration host (see "Running the application").
- **Infrastructure.Tools.DbUpdater** — standalone console app that initializes, migrates, and seeds the application database. Used both at startup (via AppHost) and as a deployable migration tool.
- **web** — React 19 + TypeScript + Vite frontend.

### Mediator (DispatchR)

This project uses **DispatchR**, not MediatR (MediatR was removed; do not reintroduce it). The signatures differ:
- Requests implement `IRequest<TRequest, TResponse>` where `TResponse` is the **full** return type, e.g. `IRequest<CreateUserRequest, Task<Result<CreateUserResult>>>`.
- Handlers implement `IRequestHandler<TRequest, Task<Result<...>>>` with `Handle(TRequest request, CancellationToken ct)`.
- Pipeline behaviors implement `IPipelineBehavior<TRequest, Task<TResponse>>` and chain via a `NextPipeline` property (not a `next` delegate). Order is set in `Application/DependencyInjection.cs` via `options.PipelineOrder`.

### Result pattern

Use cases return `Result<T>` rather than throwing for expected failures. Validation/business failures return an `Error`; success returns the value. Controllers resolve it with `result.Match(onSuccess: r => Ok(r), onFailure: e => Problem(e))`. Validation uses **FluentValidation** (`validator.InstanceIsValid(model, out var error)`).

### Identifiers

All entity IDs are `Guid`, generated sequentially. SQL Server uses a custom generator matching EF Core's `SequentialGuidValueGenerator`; other providers use `Guid.CreateVersion7()` (UUIDv7). Generate via `_dbContext.CreateSequentialGuid()` — do not call `Guid.NewGuid()` for entity IDs.

### Database users (SQL Server only)

The template models three distinct DB principals with different privileges, configured via user secrets under `Databases:Application:SqlServer:{Api,Initialization,Migrations}:ConnectionBehavior:Password`:
- **Api** — write only to the app schema, read elsewhere, no DDL.
- **Migrations** — DDL + DML on the application database.
- **Initialization** — server-level setup.

Functional tests authenticate as these restricted users, so privilege regressions surface quickly. Sqlite does not support users/schemas, so this does not apply there.

## Testing layers

Test projects follow `<Project>.Tests.{Unit|Integration|Functional}` naming, with a matching `Type` trait used by the filters above.
- **Unit** — isolated; dependencies mocked. May use EF Core in-memory/Sqlite where DbContext mocking is impractical (grouped as Unit for speed).
- **Integration** — real database via the `Testing.Integration` library.
- **Functional** — end-to-end through API endpoints, as a restricted DB user.

`Testing.Integration` builds a seeded database **template** once per run (initialize → migrate → seed), then derives each test's isolated database from it: Sqlite copies the file; SQL Server restores from a backup (local) or clones a container image (`UseContainer == true`). Provider is selected by the `DbProvider` config value; Docker is required unless using local SQL Server.

## Working with migrations

Provider libraries each own their migrations. When the model changes, add a migration to **every** active provider with a shared name. Requires the `dotnet-ef` global tool.

```powershell
dotnet ef migrations add <Name> --no-build --context ApplicationDbContext --project ./src/Infrastructure.Data.Sqlite
dotnet ef migrations add <Name> --no-build --context ApplicationDbContext --project ./src/Infrastructure.Data.SqlServer

# Check for un-migrated model changes
dotnet ef migrations has-pending-model-changes --no-build --context ApplicationDbContext --project ./src/Infrastructure.Data.Sqlite
```

Before migrations are in real use, `./tools/migrations-squash-all.ps1` deletes and recreates the `Initial` migration. See `migrations.md` for script/apply/revert commands.

## Adding a new entity

Follow `new-entity.md` precisely — it spans Domain (`SystemEntityTypes`, entity class implementing `IStaticEntityType`), Application (`IApplicationDbContext` DbSet, `LogEvents` region), test contexts, Infrastructure (`*Configuration` + `ApplicationDbContext` registration), and a migration per provider. DbSet properties are kept alphabetized; `LogEvents` entries are organized in per-entity regions with offset constants.

## Version control

Do **not** perform git commits or any other version-control write operations (no `git commit`, `git add`, `git push`, branch/tag changes, etc.). Leave all changes uncommitted in the working tree so they can be reviewed via the source-control changes view before being committed manually.

## Conventions

- Code style is enforced by a solution-wide `.editorconfig` (near-identical to Microsoft's reference). CI runs `dotnet format --verify-no-changes` and fails on any deviation — run the format verification before considering work done.
- Constructor-injected dependencies are null-checked (`?? throw new ArgumentNullException(...)`).
- Logging uses `LogEvents` integer event IDs (defined per-entity in `Application/Common/LogEvents.cs`).
