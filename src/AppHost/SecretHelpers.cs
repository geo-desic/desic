using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Desic.AppHost;

public static class SecretHelpers
{
    public static void VerifySecrets(this IDistributedApplicationBuilder builder)
    {
        var config = builder.Configuration;
        var dbUserPasswordInitialization = config.GetValue<string>(ConfigKeys.PasswordInitialization);
        var dbUserPasswordMigrations = config.GetValue<string>(ConfigKeys.PasswordMigrations);
        var dbUserPasswordApi = config.GetValue<string>(ConfigKeys.PasswordApi);
        if (builder.Environment.IsDevelopment())
        {
#pragma warning disable ASPIREUSERSECRETS001
            var userSecretsManager = builder.UserSecretsManager;
            if (string.IsNullOrEmpty(dbUserPasswordInitialization))
            {
                dbUserPasswordInitialization ??= Guid.NewGuid().ToString();
                if (!userSecretsManager.TrySetSecret(ConfigKeys.PasswordInitialization, dbUserPasswordInitialization)) throw new InvalidOperationException($"Could not set user secret key: {ConfigKeys.PasswordInitialization}");
            }
            if (string.IsNullOrEmpty(dbUserPasswordMigrations))
            {
                dbUserPasswordMigrations ??= Guid.NewGuid().ToString();
                if (!userSecretsManager.TrySetSecret(ConfigKeys.PasswordMigrations, dbUserPasswordMigrations)) throw new InvalidOperationException($"Could not set user secret key: {ConfigKeys.PasswordMigrations}");
            }
            if (string.IsNullOrEmpty(dbUserPasswordApi))
            {
                dbUserPasswordApi ??= Guid.NewGuid().ToString();
                if (!userSecretsManager.TrySetSecret(ConfigKeys.PasswordApi, dbUserPasswordApi)) throw new InvalidOperationException($"Could not set user secret key: {ConfigKeys.PasswordApi}");
            }
#pragma warning restore ASPIREUSERSECRETS001
        }
        else
        {
            if (string.IsNullOrEmpty(dbUserPasswordInitialization)) throw new InvalidOperationException($"Required configuration value is null or empty: {ConfigKeys.PasswordInitialization}");
            if (string.IsNullOrEmpty(dbUserPasswordMigrations)) throw new InvalidOperationException($"Required configuration value is null or empty: {ConfigKeys.PasswordMigrations}");
            if (string.IsNullOrEmpty(dbUserPasswordApi)) throw new InvalidOperationException($"Required configuration value is null or empty: {ConfigKeys.PasswordApi}");
        }
    }
}
