# Application Project

This project is the [application](https://github.com/jasontaylordev/CleanArchitecture/tree/main/src/Application) (aka use cases) layer.
- Application specific (not enterprise wide) use cases
- Orchestrating the flow of data to and from the domain layer
- Example Contents
  - Application services and use case handlers (e.g. [CreateUserRequestHandler.cs](Users/Create/CreateUserRequestHandler.cs))
  - Data transfor objects (DTOs) (e.g. [User](Users/User.cs))
  - Application specific rules and validation (e.g. [CreateUserValidator.cs](Users/Create/CreateUserValidator.cs))

This library is not for defining any persistence formats which are in [Infrastructure.Data.Configurations](../Infrastructure/Data/Configurations).

This library depends on [Domain](../Domain) but should **not** depend on any outer layers: [Infrastructure](../Infrastructure), [Api](../Api), or [Web](../Web).