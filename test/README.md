# Tests
In visual studio, can use the Test Explorer to execute any desired tests. Can also use its group by functionality to easily execute only the desired types of tests: Unit, Integration, Functional.

Can also execute the tests from a terminal in the top level solution directory using a command such as:
```
dotnet test
```
```
dotnet test --ignore-exit-code 8 -- --filter-trait "Type=Unit"
# --ignore-exit-code 8 ===> so errors will not be generated if 0 tests are found for a specific test project due to the trait filter
```

There is also [dotnet-test-with-coverage-report.ps1](../tools/dotnet-test-with-coverage-report.ps1) which demonstrates how to execute all tests generating a code coverage report. Afterward can view the report in a browser by navigating to the created coverage-report/index.html file.

## Unit Tests
Unit tests validate an individual unit (e.g. function, method, class) works in isolation. Any external dependencies should be mocked or stubbed. In some cases in-memory database providers (entity framework core / sqlite) are used since mocking DbSets / DbContexts have various issues and do not support all types of operations. Such tests may technically be integration tests (or closer to them depending on how many other dependencies are mocked), but since they execute significantly faster, grouping them with unit for performance reasons makes sense. Unit test projects have the naming convention `*.Tests.Unit` where `*` is the name of the project containing the unit being tested.

## Integration Tests
Integration tests validate multiple units/modules work when combined as a group. These often use real (or production-like) depdendencies. For example many of these use a fully functional database as opposed to an in memory one that may not accurately reflect production behavior. Integration test projects (that are not functional tests) have the naming convention `*.Tests.Integration` where `*` is the name of the project containing the module being tested.

## Functional Tests
Functional tests are a specific type of integration test that validate the module works from the end-user's perspective according to any applicable business requirements, e.g. testing an API endpoint end to end. These often require a fully running (or nearly complete) system and environment. Functional tests have the naming convention `*.Tests.Functional` where `*` is the name of the project containing the module being tested.

## Test Database
During unit tests the database is either mocked or an in memory provider is used. For integration tests (including functional) an actual database is used. See the Testing.Integration project [readme](Testing.Integration/README.md) for more information on this.