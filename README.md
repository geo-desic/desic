# Desic Solution

## Initial Setup For Development

### Required Tools
While it is recommended to install both of the following, at least one will be needed to set up the database for development and testing:
- [Docker desktop](https://www.docker.com/products/docker-desktop/)
  - required to utilize the aspire functionality
- Locally running sql server such as [LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) or [Sql Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
  - LocalDB (with default service name `MSSQLLocalDB`) is currently configured as default when individual projects (e.g. [Api](src/Api), [Infrastructure.Tools.DbUpdater](src/Infrastructure.Tools.DbUpdater)) are launched

### User Secrets
The projects use [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) to store sensitive information during development.

#### Automatic Configuration
Simply run the AppHost project. It currently has support for creating the necessary user secrets if they do not exist.

#### Manual User Secret Configuration

Right click on the Api project in Visual Studio and select "Manage User Secrets". This will open a `secrets.json` file where you can add your secrets. There may be a few already present and if there are don't remove them. But you will likely need to add some additional ones.

The following is an example (not to be used as is) of what your `secrets.json` file should look like after adding the necessary secrets. You can use a GUID generator (or your preferred secure password generator) to assist in creating the passwords.

```json
{
  \\ potentially other secrets may already be present, but append the following to the file replacing the values with your own
  "Databases:Application:SqlServer:Api:ConnectionBehavior:Password": "00000000-0000-0000-0000-000000000001",
  "Databases:Application:SqlServer:Initialization:ConnectionBehavior:Password": "00000000-0000-0000-0000-000000000002",
  "Databases:Application:SqlServer:Migrations:ConnectionBehavior:Password": "00000000-0000-0000-0000-000000000003"
}
```

## Running The Application

- (Recommended) Run the [AppHost](src/AppHost) startup project using the `https` launch profile
  -  this project has support for starting up all necessary dependencies in the correct order and waiting for each one to be ready
- Individual projects (e.g. [Api](src/Api), [Infrastructure.Tools.DbUpdater](src/Infrastructure.Tools.DbUpdater)) can also be launched, but this requires that their dependencies are started and ready
  - for example the Api depends on the database service being available and the application database on that server existing, online, initialized, migrated, and seeded properly