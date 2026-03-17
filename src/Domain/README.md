# Domain Project

This project is the [domain](https://github.com/jasontaylordev/CleanArchitecture/tree/main/src/Domain) (aka [core](https://devblogs.microsoft.com/ise/next-level-clean-architecture-boilerplate/#the-core-layer)) layer.
- Core enterprise wide business rules
  - remain contant regardless of the application's context or the technology stack used
- Example Contents
  - Abstraction interfaces
  - Domain entities (e.g. [User](Users/User.cs))
    - This does not include any persistence concerns (e.g. database table definitions) which are in [Infrastructure.Data.Configurations](../Infrastructure/Data/Configurations)
    - Note that these domain entities in many cases will not be the same as what is accepted/returned from the API. The API data can be reshaped (both incoming and outgoing) by data transfer objects.
  - Domain events/services
  - Enums
  - Exception classes

This library is not for **handling** requests to get or persist data. Those handlers should be in the [Application](../Application) or [Infrastructure](../Infrastructure) projects.

This library should **not** depend on any outer layers: [Application](../Application), [Infrastructure](../Infrastructure), [Api](../Api), or [web](../web).