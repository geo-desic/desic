using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

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
    const string configKeyDbUserPasswordInitialization = "Databases:Application:InitializationUserPassword";
    const string configKeyDbUserPasswordMigrations = "Databases:Application:MigrationsUserPassword";
    const string configKeyDbUserPasswordApi = "Databases:Application:ApiUserPassword";
    var dbUserPasswordInitialization = config.GetValue<string>(configKeyDbUserPasswordInitialization);
    var dbUserPasswordMigrations = config.GetValue<string>(configKeyDbUserPasswordMigrations);
    var dbUserPasswordApi = config.GetValue<string>(configKeyDbUserPasswordApi);
    if (builder.Environment.IsDevelopment())
    {
#pragma warning disable ASPIREUSERSECRETS001
        var userSecretsManager = builder.UserSecretsManager;
        if (string.IsNullOrEmpty(dbUserPasswordInitialization))
        {
            dbUserPasswordInitialization ??= Guid.NewGuid().ToString();
            if (!userSecretsManager.TrySetSecret(configKeyDbUserPasswordInitialization, dbUserPasswordInitialization)) throw new InvalidOperationException($"Could not set user secret key: {configKeyDbUserPasswordInitialization}");
        }
        if (string.IsNullOrEmpty(dbUserPasswordMigrations))
        {
            dbUserPasswordMigrations ??= Guid.NewGuid().ToString();
            if (!userSecretsManager.TrySetSecret(configKeyDbUserPasswordMigrations, dbUserPasswordMigrations)) throw new InvalidOperationException($"Could not set user secret key: {configKeyDbUserPasswordMigrations}");
        }
        if (string.IsNullOrEmpty(dbUserPasswordApi))
        {
            dbUserPasswordApi ??= Guid.NewGuid().ToString();
            if (!userSecretsManager.TrySetSecret(configKeyDbUserPasswordApi, dbUserPasswordApi)) throw new InvalidOperationException($"Could not set user secret key: {configKeyDbUserPasswordApi}");
        }
#pragma warning restore ASPIREUSERSECRETS001
    }
    else
    {
        if (string.IsNullOrEmpty(dbUserPasswordInitialization)) throw new InvalidOperationException($"Required configuration value is null or empty: {configKeyDbUserPasswordInitialization}");
        if (string.IsNullOrEmpty(dbUserPasswordMigrations)) throw new InvalidOperationException($"Required configuration value is null or empty: {configKeyDbUserPasswordMigrations}");
        if (string.IsNullOrEmpty(dbUserPasswordApi)) throw new InvalidOperationException($"Required configuration value is null or empty: {configKeyDbUserPasswordApi}");
    }

    var password = builder.AddParameter(name: "sqlserver-password-sa", value: dbUserPasswordInitialization, secret: true);
    database = builder.AddSqlServer("sqlserver-resource", password: password);

    callbackArgsDbUpdater = async (c) =>
    {
        var connectionStringInitialization = await database.Resource.GetConnectionStringAsync().ConfigureAwait(false) ?? throw new InvalidOperationException("Could not resolve database connection string");
        var connectionStringMigrations = new SqlConnectionStringBuilder(connectionStringInitialization) { InitialCatalog = "Desic", UserID = "migrations", Password = dbUserPasswordMigrations }.ConnectionString;
        c.Args.Add("--ci");
        c.Args.Add(connectionStringInitialization);
        c.Args.Add("--c");
        c.Args.Add(connectionStringMigrations);
        c.Args.Add("--Databases:Application:SqlServer:StopIfExists");
        c.Args.Add("false");
    };

    callbackEnvironmentApi = async (c) =>
    {
        var connectionStringInitialization = await database.Resource.GetConnectionStringAsync().ConfigureAwait(false) ?? throw new InvalidOperationException("Could not resolve database connection string");
        var connectionStringApi = new SqlConnectionStringBuilder(connectionStringInitialization) { InitialCatalog = "Desic", UserID = "api", Password = dbUserPasswordApi }.ConnectionString;
        c.EnvironmentVariables.Add("ConnectionStrings__SqlServer", connectionStringApi);
    };

    projectResourceDbUpdater.WaitFor(database);
}
else if (dbProvider == "Sqlite")
{
    database = builder.AddSqlite("Sqlite", databaseFileName: "desic.db");

    callbackArgsDbUpdater = async (c) =>
    {
        var connectionStringMigrations = await database.Resource.GetConnectionStringAsync().ConfigureAwait(false) ?? throw new InvalidOperationException("Could not resolve database connection string");
        c.Args.Add("--c");
        c.Args.Add(connectionStringMigrations);
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
