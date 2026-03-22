# Entity Framework Migrations
All commands below assume a terminal in the top level solution directory. The entity framework cli tools are required and can be installed/updated using the following commands. The tools should be kept relatively up to date. A warning will likely be issued with an out of date version when creating a new migration.

```powershell
# initial install
dotnet tool install --global dotnet-ef
# update to latest version
dotnet tool update --global dotnet-ef
```

## Determine If There Are Pending Model Changes
```powershell
dotnet ef migrations has-pending-model-changes --no-build --context ApplicationDbContext --project ./src/Infrastructure.Data.Sqlite
dotnet ef migrations has-pending-model-changes --no-build --context ApplicationDbContext --project ./src/Infrastructure.Data.SqlServer
```

## Create A Migration
Choose a (mostly) unique name for the migration, e.g. `PersonEntityAdded`, and execute a `dotnet ef migrations add` command for all active providers.

```powershell
dotnet ef migrations add <MigrationName> --no-build --context ApplicationDbContext --project ./src/Infrastructure.Data.Sqlite
dotnet ef migrations add <MigrationName> --no-build --context ApplicationDbContext --project ./src/Infrastructure.Data.SqlServer
```

## Migrations Script
```powershell
dotnet ef migrations script --no-build --context ApplicationDbContext --project ./src/Infrastructure.Data.Sqlite -o migration-sqlite.sql
dotnet ef migrations script --no-build --context ApplicationDbContext --project ./src/Infrastructure.Data.SqlServer -o migration-sqlsever.sql
```

## Applying Migrations To A Database
This would most likely not be done using the cli ef tools and rather the `DbUpdater` application or a migrations script. However ef tools commands below for reference.

```powershell
dotnet ef database update --no-build --context ApplicationDbContext --connection "Data Source=./desic.db;" --project ./src/Infrastructure.Data.Sqlite
dotnet ef database update --no-build --context ApplicationDbContext --connection "Server=(localdb)\MSSQLLocalDB;Database=Desic;Trusted_Connection=True;" --project ./src/Infrastructure.Data.SqlServer
```

## Reverting Migrations From A Database

```powershell
# Sqlite
# revert to <prior-migration-name>
dotnet ef database update <prior-migration-name> --no-build --context ApplicationDbContext --connection "Data Source=./desic.db;" --project ./src/Infrastructure.Data.Sqlite
# revert to beginning (0)
dotnet ef database update 0 --no-build --context ApplicationDbContext --connection "Data Source=./desic.db;" --project ./src/Infrastructure.Data.Sqlite

# SqlServer
# revert to <prior-migration-name>
dotnet ef database update <prior-migration-name> --no-build --context ApplicationDbContext --connection "Server=(localdb)\MSSQLLocalDB;Database=Desic;Trusted_Connection=True;" --project ./src/Infrastructure.Data.SqlServer
# revert to beginning (0)
dotnet ef database update 0 --no-build --context ApplicationDbContext --connection "Server=(localdb)\MSSQLLocalDB;Database=Desic;Trusted_Connection=True;" --project ./src/Infrastructure.Data.SqlServer
```