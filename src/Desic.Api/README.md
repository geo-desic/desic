# Desic API

## Dependency Chain

| Project(s)                |
|---------------------------|
| Desic.Api                 |
| Desic.Business            |
| Desic.EntityFrameworkCore |

All of the above can have direct and/or indirect references to:

| Project                   |
|---------------------------|
| Desic                     |
| MediatR                   |
| MediatR.Contracts         |

This API is designed not to have any dependencies on a specific entity framework provider project (e.g. Desic.EntityFrameworkCore.SqlServer). Those are currently only used by the database updater project to apply migrations to a database and/or create it. The `DbProvider` configuration element can be used to control the desired provider.