# Desic API

## Dependency Chain

| Project(s)                |
|---------------------------|
| Desic.Api                 |
| Desic.Business            |
| Desic.EntityFrameworkCore |
| Desic                     |
| MediatR                   |
| MediatR.Contracts         |

This is designed not to have any dependencies on the specific entity framework provider projects (e.g. Desic.EntityFrameworkCore.SqlServer). Those are currently only used by the database updater project to apply migrations to a specific database and/or create the initial database. The `DbProvider` configuration element can be used to control the desired provider.