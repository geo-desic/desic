# To Do List

- [ ] Add some entity types requiring joins / navigation properties to determine if there are any significant gaps with the existing infrastructure
- [ ] Authentication design
  - [ ] Authentication implementation
- [ ] Authorization design
  - [ ] Authorization implementation
- [ ] Decide if validation behavior should be added
  - [ ] If so, implement validation behavior
- [ ] Decide if filtering/ordering/pagination infrastructure should be enhanced to generic classes with interface constraints so it can be used with projections
  - [ ] If so, implement filtering/ordering/pagination enhancements
- [ ] Verify and enhance openapi documentation
- [ ] Implement support for a consistent Guid generation method/delegate for non predefined data
  - [ ] Specifically, use a [byte shuffled UUIDv7](src/Shared/Extensions/GuidExtensions.cs) for all sql server ids
- [ ] Determine if the Guid generation method for seeded predefined data (e.g. [SystemEntityTypes](src/Domain/EntityTypes/SystemEntityTypes.cs), predefined [TestUsers](src/Domain/Users/Test/TestUsers.cs)) should be changed to conform with [UUIDv8](https://www.rfc-editor.org/rfc/rfc9562#name-uuid-version-8) (i.e. set the version and variant bits)
  - [ ] If so, implement the generation method