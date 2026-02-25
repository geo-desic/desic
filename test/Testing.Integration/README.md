# Integration Testing Core Library

All integration tests require a database provider. The provider is set using the configuration element `DbProvider` which currently supports the following values.
- `Sqlite`
- `SqlServer`

## Required Tools

### For all providers except `DbProvider = Sqlite`

[Docker Engine](https://docs.docker.com/engine/install/) or [Docker desktop](https://www.docker.com/products/docker-desktop/)
- Used when `DbProviders:<ActiveDbProvider>:UseContainer = true`

### For `DbProvider = SqlServer`

[LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)
- Used when `DbProviders:SqlServer:UseContainer = false`