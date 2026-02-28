# Database Updater
This application can be used to initialize a database and apply migrations to it. It is designed to be run from the command line, and can be used in conjunction with a CI/CD pipeline to automate database updates.

## Database Initialization
This is the process of creating a new database and/or performing any initial configuration to it such as creating schemas, roles, and users (see [sqlserver.appsettings.json](../Infrastructure.Data.SqlServer/sqlserver.appsettings.json)). This is typically done when setting up a new environment for the first time. Note that this may (and currently does) include creating a migrations database user specifially for applying migrations to it. So this step, if performed, occurs before applying any migrations.

## Migrations
This is the process of applying changes to an existing database. Migrations are typically used to evolve the database over time as the application requirements change. This can include things such as adding/modifying/removing tables, columns, indexes, and constraints.

## Command line arguments
| Option | Short | Description |
| ------ | ----- | ----------- |
| `--connection-init <CONNECTION>` | `--ci`  | The connection string for database initialization. The word `migrations` (case-sensitive) can be used for `<CONNECTION>` to use the same connection string as migrations (i.e. `-c`). If not provided database initialization will be skipped. |
| `--connection <CONNECTION>` | `--c` | The connection string for applying migrations. If not provided, applying migrations will be skipped. |
| `--no-seeding` | `--ns` | Skip seeding the database after applying migrations. By default, the database will be seeded after applying migrations. |

See launchSettings.json for examples of how to use these options when running the application from Visual Studio.