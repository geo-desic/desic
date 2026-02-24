# Desic Core Project

This project is the [core](https://devblogs.microsoft.com/ise/next-level-clean-architecture-boilerplate/#the-core-layer) (aka [domain](https://github.com/jasontaylordev/CleanArchitecture/tree/main/src/Domain)) layer mainly for defining the following.
- [Request](Requests) and response **format** to get or manipulate data, e.g.
  - Get a user by their id or username
  - Update the username for a user
- Data **format**
  - The [Entities](Entities) folder data is essentially the same as it will persisted in the data provider (e.g. database table definition)
    - Note that this data format in many cases will not be the same as what is returned from the API. The data can be reshaped potentially from multiple entities before being returned in a data transfer object from the API.
- Abstraction interfaces (e.g. [IRepository](Shared/IRepository.cs))
- Exception classes
- Enums (e.g. [SystemEntityType](EntityTypes/SystemEntityType.cs))

This library is not for **handling** requests to get or manipulate data. Those handlers should be in the application or infrastructure projects. The only exceptions are requests for data that does not require business logic or a data provider. Some examples of that are the following.
- [Query handlers](Handlers/Queries) for data that does not require a data provider such as getting the records in an embedded resource, e.g. [iso-3166-countries.csv](Iso3166Countries/iso-3166-countries.csv)
- Generation (but not persistence) of [Test](Users/Test) data

This library should not have project references nor dependencies to the outer layers: Application, Infrastructure, Api, Web