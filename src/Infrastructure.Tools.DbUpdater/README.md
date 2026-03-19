# Database Updater
This application can be used to initialize a database and/or apply migrations/seed data to it. It is designed to be run from the command line and can be used in conjunction with a CI/CD pipeline to automate database updates.

## Database Initialization
This is the process of creating a new database and/or performing any initial configuration to it such as creating schemas, roles, and users (see [sqlserver.appsettings.json](../Infrastructure.Data.SqlServer/sqlserver.appsettings.json)). This is typically done when setting up a new environment for the first time. Note that this may (and currently does) include creating a migrations database user specifially for applying migrations to it. So this step, if performed, occurs before applying any migrations.

## Migrations
This is the process of applying changes to an existing database. Migrations are typically used to evolve the database over time as the application requirements change. This can include things such as adding/modifying/removing tables, columns, indexes, and constraints.

## Seeding
This is the process of inserting/updating/deleting specific data in the database that likely needs to be present for the application to function correctly (or in some cases facilitate testing).

## Command line arguments
| Option | Short | Description |
| ------ | ----- | ----------- |
| `--connection-init <CONNECTION>` | `-ci`  | The **exact** connection string for database initialization. The word `migrations` (case-sensitive) can be used for `<CONNECTION>` to use the same connection string as migrations (i.e. `-c`). No alterations to this connection string will be performed. If initialization is enabled and this is not provided, the connection string will be created from configuration with potential alterations such as substituting a userid or password. |
| `--connection <CONNECTION>` | `-c` | The **exact** connection string for applying migrations. No alterations to this connection string will be performed. If migrations are enabled and this is not provided, the connection string will be created from configuration with potential alterations such as substituting a userid or password. |
| `--provider <PROVIDER>` | `-p` | The database provider to use. Currently only 2 values are supported `Sqlite` and `SqlServer`. If not set, the `DbProvider` configuration element will be used. |
