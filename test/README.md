# Tests

## Unit Tests
Unit tests validate an individual unit (e.g. function, method, class) works in isolation. Any external dependencies should be mocked or stubbed. In some cases in-memory database providers (entity framework core / sqlite) are used since mocking DbSets / DbContexts do not support all types of operations. Such tests may technically be closer to integration tests, but since they execute significantly faster, grouping them with unit for performance reasons makes sense. Unit test projects have the naming convention `*.Tests.Unit` where `*` is the name of the project containing the unit being tested.

## Integration Tests
Integration tests validate multiple units/modules work when combined as a group. These often use real (or production-like) depdendencies. For example many of these might use a fully functional database as opposed to an in memory database that does not accurately reflect production behavior. Integration test projects (that are not functional tests) have the naming convention `*.Tests.Integration` where `*` is the name of the project containing the module being tested.

## Functional Tests
Functional tests are a specific type of integration test that validate the module works from the end-user's perspective according to any applicable business requirements, e.g. testing an API endpoint end to end. These often require a fully running (or nearly complete) system and environment. Functional tests have the naming convention `*.Tests.Functional` where `*` is the name of the project containing the module being tested.

## Test Database
During unit tests the database is either mocked or in some cases the EF in memory provider is used. For integration tests (including functional) an actual database is used. See the Testing.Integration project [readme](Testing.Integration/README.md) for more information.