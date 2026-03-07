# Desic Solution

## Initial Setup For Development

### Required Tools
While it is recommended to install both of the following, at least one will be needed to set up the database for development:
- [LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)
- [Docker desktop](https://www.docker.com/products/docker-desktop/)
  - this is required to utilize the aspire functionality

### User Secrets
The projects use [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) to store sensitive information during development. To set up your own user secrets, the simplest way is to run the AppHost project. It currently has support for creating the necessary user secrets if they do not exist. However, you can also manually create them as shown below.

#### Manual User Secret Configuration

Right click on the Api project in Visual Studio and select "Manage User Secrets". This will open a `secrets.json` file where you can add your secrets. There may be a few already present and if there are don't remove them. But you will likely need to add some additional ones as described below.

The following is an example (not to be used as is) of what your `secrets.json` file should look like after adding the necessary secrets. You can use a GUID generator (or your preferred secure password generator) to assist in creating the passwords.

```json
{
  \\ potentially other secrets may already be present, but append the following to the file replacing the values with your own
  "Databases:Application:SqlServer:InitializationPassword": "00000000-0000-0000-0000-000000000001",
  "Databases:Application:SqlServer:Users:Api:Password": "00000000-0000-0000-0000-000000000002",
  "Databases:Application:SqlServer:Users:Migrations:Password": "00000000-0000-0000-0000-000000000003"
}
```