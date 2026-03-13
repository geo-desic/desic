using Desic.AppHost;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var config = builder.Configuration;
var dbProvider = builder.Configuration.GetValue<string>("DbProvider") ?? throw new InvalidOperationException("DbProvider could not be determined from configuration");
IResourceBuilder<IResourceWithConnectionString>? database = null;
Func<CommandLineArgsCallbackContext, Task> callbackArgsDbUpdater;
Func<EnvironmentCallbackContext, Task> callbackEnvironmentApi;

var projectResourceDbUpdater = builder.AddProject<Projects.Infrastructure_Tools_DbUpdater>("db-updater", launchProfileName: null)
    .WithEnvironment("DOTNET_ENVIRONMENT", builder.Environment.EnvironmentName)
    .WithEnvironment("DbProvider", dbProvider);

if (dbProvider == "SqlServer")
{
    builder.VerifySecrets();
    var dbUserPasswordInitialization = config.GetValue<string>(ConfigKeys.PasswordInitialization)!;

    var password = builder.AddParameter(name: "sqlserver-password-sa", value: dbUserPasswordInitialization, secret: true);
    database = builder.AddSqlServer("sqlserver-resource", password: password);

    callbackArgsDbUpdater = async (c) =>
    {
        var connectionString = await database.Resource.GetConnectionStringAsync().ConfigureAwait(false) ?? throw new InvalidOperationException("Could not resolve database connection string");
        var connectionStringDatabase = new SqlConnectionStringBuilder(connectionString) { InitialCatalog = "Desic", UserID = string.Empty, Password = string.Empty }.ConnectionString;
        c.Args.Add($"--ConnectionStrings:SqlServer={connectionStringDatabase}");
    };

    callbackEnvironmentApi = async (c) =>
    {
        var connectionString = await database.Resource.GetConnectionStringAsync().ConfigureAwait(false) ?? throw new InvalidOperationException("Could not resolve database connection string");
        var connectionStringDatabase = new SqlConnectionStringBuilder(connectionString) { InitialCatalog = "Desic", UserID = string.Empty, Password = string.Empty }.ConnectionString;
        c.EnvironmentVariables.Add("ConnectionStrings__SqlServer", connectionStringDatabase);
    };

    projectResourceDbUpdater.WaitFor(database);
}
else if (dbProvider == "Sqlite")
{
    database = builder.AddSqlite("Sqlite", databaseFileName: "desic.db");

    callbackArgsDbUpdater = async (c) =>
    {
        var connectionString = await database.Resource.GetConnectionStringAsync().ConfigureAwait(false) ?? throw new InvalidOperationException("Could not resolve database connection string");
        c.Args.Add($"--ConnectionStrings:Sqlite={connectionString}");
    };

    callbackEnvironmentApi = async (c) =>
    {
        var connectionStringApi = await database.Resource.GetConnectionStringAsync().ConfigureAwait(false) ?? throw new InvalidOperationException("Could not resolve database connection string");
        c.EnvironmentVariables.Add("ConnectionStrings__Sqlite", connectionStringApi);
    };

    projectResourceDbUpdater.WaitFor(database);
}
else
{
    throw new InvalidOperationException($"Unsupported database provider: {dbProvider}");
}

projectResourceDbUpdater
    .WithArgs(callbackArgsDbUpdater);

var projectResourceApi = builder.AddProject<Projects.Api>("api")
    .WithEnvironment("DbProvider", dbProvider)
    .WithEnvironment(callbackEnvironmentApi)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName)
    .WaitFor(database)
    .WaitForCompletion(projectResourceDbUpdater);

builder.AddViteApp(name: "web-resource", appDirectory: "../web")
    .WithReference(projectResourceApi)
    .WaitFor(projectResourceApi)
    .WithNpm();

builder.Build().Run();
