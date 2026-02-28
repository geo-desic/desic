# API Project

## Dependency Chain

| Project(s)                |
|---------------------------|
| Api                       |
| Infrastructure            |
| Application               |
| Domain                    |

All of the above can have direct and/or indirect references to:

| Project                   |
|---------------------------|
| Desic                     |
| MediatR                   |
| MediatR.Contracts         |

This API is designed not to have any dependencies on a specific entity framework provider project (e.g. Infrastructure.Data.SqlServer). Those are currently only used by the database updater and integration test projects to apply migrations to a database and/or create it. The `DbProvider` configuration element can be used to control the desired provider.