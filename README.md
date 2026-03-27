# Desic Solution

This is currently a template for a new .net api following recommended/best practices.

- [Quickstart - Development](development.md)

## Design
- Aspire support for simplifying setup and development process
- Follows domain driven design (DDD) and clean architecture principles similar to: [ardalis/CleanArchitecture](https://github.com/ardalis/CleanArchitecture), [jasontaylordev/CleanArchitecture](https://github.com/jasontaylordev/cleanarchitecture), [microsoft-next-level-boilerplate](https://devblogs.microsoft.com/ise/next-level-clean-architecture-boilerplate/)
- Independent of a specific database provider. Can be customized to work with any major database provider with minimal changes. Two examples, `Sqlite` and `SqlServer`, are currently included. For other providers a dedicated class library similar to [Infrastructure.Data.SqlServer](src/Infrastructure.Data.SqlServer) could be created.
- Robust [testing framework](test/README.md) with a high degree of code coverage and multiple types: Unit, Integration, and Functional
- Extensive common infrastructure for promoting consistency and simplifying implementation ([DRY principle](https://en.wikipedia.org/wiki/Don%27t_repeat_yourself))
  - [Domain.Common](src/Domain/Common)
  - [Application.Common](src/Application/Common)
  - [Infrastructure.Data.Common](src/Infrastructure/Data/Common)
  - [Api.Common](src/Api/Common)
  - [Testing.Integration](test/Testing.Integration) - used by all integration/functional test projects
- Designed with vertical slicing in mind, e.g.
  - [Domain/EntityTypes](src/Domain/EntityTypes)
  - [Application/EntityTypes](src/Application/EntityTypes)
  - [Infrastructure.Data/EntityTypes](src/Infrastructure/Data/EntityTypes)
  - [Api.Controllers.V1.EntityTypesController.cs](src/Api/Controllers/V1/EntityTypesController.cs)
    - Could be modified to a [minimal api](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-10.0) or [fast endpoint](https://fast-endpoints.com) implementation if segregating each action is desired
- Includes support for multiple different types of database authentication, e.g.
  - dedicated api database user with restricted access: no ddl and write access only to a specific schema in the application database (read only elsewhere)
  - dedicated migrations database user with more permissive access: ddl and dml permissions to the application database
  - currently implemented by the functional testing framework so any potential issues with new code should be identified quickly
  - note that this is not supported by the Sqlite infrastructure as it does not support database users, schemas, etc.
- Multiple [health checks](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks) implemented with [test coverage](test/Api.Tests.Functional/HealthCheckTests.cs)
- No sensitive data checked into source control
  - [user secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) are used for development purposes which would be handled by desired secret manager for other environments, e.g. azure keyvault
  - see the [development guide](development.md) for more information