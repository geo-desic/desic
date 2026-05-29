# Infrastructure Project

This project is the [infrastructure](https://github.com/jasontaylordev/CleanArchitecture/tree/main/src/Infrastructure) layer.

- Handles external concerns such as
  - Database provider implementations and persistence format
  - Messaging systems
  - Email services

This library should not have provider specific database initialization or migration code. Those should be in a provider specific library such as [Infrastructure.Data.Sqlite](../Infrastructure.Data.Sqlite) or [Infrastructure.Data.SqlServer](../Infrastructure.Data.SqlServer).

This depends on both [Domain](../Domain) and [Application](../Application) but it should not depend on [Api](../Api) or [web](../web).