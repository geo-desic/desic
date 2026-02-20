# Desic Solution

## Initial Setup For Development

### Required Tools
While it is recommended to install both of the following, at least one will be needed to set up the database for development:
- [LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)
- [Docker desktop](https://www.docker.com/products/docker-desktop/)
  - this is required to utilize the docker-compose functionality

### User Secrets
The projects use [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) to store sensitive information during development. To set up your own user secrets, follow these steps:

1. Right click on the project in Visual Studio and select "Manage User Secrets". This will open a `secrets.json` file where you can add your secrets. There may be a few already present, but you will likely need to add some additional ones.

The following is an example (not to be used as is) of what your `secrets.json` file should look like after adding the necessary secrets. You can use a GUID generator to assist in creating the passwords.

```json
{
  \\ potentially other secrets may already be present, but append the following to the file replacing the GUIDs with your own
  "Databases:Desic:ApiUserPassword": "00000000-0000-0000-0000-000000000001",
  "Databases:Desic:InitializationUserPassword": "00000000-0000-0000-0000-000000000002",
  "Databases:Desic:MigrationsUserPassword": "00000000-0000-0000-0000-000000000003"
}
```