# Integration Testing Library

All integration tests require a database provider. The provider is set using the configuration element `DbProvider` which currently supports the following values.
- `Sqlite`
- `SqlServer`

## Required Tools

[Docker Engine](https://docs.docker.com/engine/install/) or [Docker desktop](https://www.docker.com/products/docker-desktop/)
- `DbProvider != Sqlite`
- Used when `DbProviders:<ActiveDbProvider>:UseContainer == true`

Locally running SQL Server such as [LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) or [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- `DbProvider == SqlServer`
- Used when `DbProviders:SqlServer:UseContainer == false`
- `ConnectionStrings.SqlServer` must be set properly in `appsettings.Test.json` to connect to the local server's master database

## Test Database
At the very beginning of the test run a "template" database is created (using an [xunit assembly fixture](https://xunit.net/docs/shared-context#assembly-fixture)) which performs the following steps:
1. database initialization (if applicable)
2. database migrations
3. database seeding

From this point forward the template is used to create any databases the for use by tests. Using this flow the steps above are performed only once per test run. The exact method for creating the test database from the template depends on the configuration.

### Sqlite
Each test database is created from the template simply by copying the template database file. The template and test databases have a guid component as part of their name to prevent database name conflicts and allow multiple test databases to be used in parallel. All template and test database files are stored locally in a temporary directory and cleaned up at the end of the test run.

### SqlServer - Local
When `DbProviders:SqlServer:UseContainer == false` a locally running sql server is used. At the end of the template creation process the database is backed up. Then each test database is restored using this backup. The template and test databases have a guid component as part of their name to prevent database name conflicts and allow multiple test databases to be used in parallel. The template backup (\*.bak) and all test database files (\*.mdf, \*.ldf) are stored locally in a temporary directory and cleaned up at the end of the test run.

### SqlServer - Container
When `DbProviders:SqlServer:UseContainer == true` a container is used for the template and each test database. The `DbProviders:SqlServer:ContainerImage` value is used to create the template container. Once the template database has been initialized, migrated, and seeded its sql server is shutdown and the container stopped. Then a temporary docker image of the template container is created. Each test database container is created using this template image. The temporary image and all containers are cleaned up at the end of the test run.
