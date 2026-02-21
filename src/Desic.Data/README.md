# Desic Data Library

This library is mainly for defining the following.
- [Request](Requests) and response format to get or manipulate data, e.g.
  - Get a user by their id or username
  - Update the username for a user
- Data format
  - The [Entities](Entities) folder data is essentially the same as it will persisted in the data provider (e.g. database table definition)
    - Note that this data format in many cases will not be the same as what is returned from the API. For example a [User](Entities/User.cs) record may only have minimal information such as a username. The API may want to return more data including from other entities, such as the person's name, addresses, phone numbers, etc. In fact the API may have multiple ways of getting the data for a user, perhaps a minimal one and a much more detailed one.

This library is not for **handling** requests to get or manipulate data. Those handlers should be in the provider project, currently `Desic.Data.EFCore`. The only exceptions are requests for data that does not require a provider. Some examples of that are the following.
- [Query handlers](Handlers/Queries) for data that does not require a data provider such as getting the records in an embedded resource, e.g. [iso-3166-countries.csv](Resources/iso-3166-countries.csv)
- Generation (but not persistence) of [Test](test) data