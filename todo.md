# To Do List

- [ ] Add some entity types requiring joins / navigation properties to determine if there are any significant gaps with the existing infrastructure
- [ ] Authentication design
  - [ ] Authentication implementation
- [ ] Authorization design
  - [ ] Authorization implementation
- [ ] Decide if validation behavior should be added
  - [ ] If so, implement validation behavior
- [ ] Verify and enhance openapi documentation
- [ ] Determine if the Guid generation method for seeded predefined data (e.g. [SystemEntityTypes](src/Domain/EntityTypes/SystemEntityTypes.cs), predefined [TestUsers](src/Domain/Users/Test/TestUsers.cs)) should be changed to conform with [UUIDv8](https://www.rfc-editor.org/rfc/rfc9562#name-uuid-version-8) (i.e. set the version and variant bits)
  - [ ] If so, implement the generation method
- [ ] Implement some basic github workflows for the repository
  - [ ] CI workflow for running tests and code analysis
  - [ ] CD workflow for deploying to a staging environment