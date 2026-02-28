# Desic Core Project

This project is the [domain](https://github.com/jasontaylordev/CleanArchitecture/tree/main/src/Domain) (aka [core](https://devblogs.microsoft.com/ise/next-level-clean-architecture-boilerplate/#the-core-layer)) layer mainly for defining the following.
- Data **format** for domain entities (e.g. [User](Users/User.cs))
  - This does not include any persistence concerns (e.g. database table definitions) which are in the Infrastructure project (Data/Configurations)
  - Note that this data format in many cases will not be the same as what is returned from the API. The data can be reshaped (both incoming and outgoing) by data transfer objects which will be used in API.
- Abstraction interfaces
- Exception classes
- Enums (e.g. [SystemEntityType](EntityTypes/SystemEntityType.cs))

This library is not for **handling** requests to get or manipulate data. Those handlers should be in the application or infrastructure projects.

This library should **not** have project references nor dependencies to the outer layers: Application, Infrastructure, Api, or Web