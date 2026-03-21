using AwesomeAssertions;
using Desic.Testing.Integration.Db;
using Desic.Testing.Integration.Hosting;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Desic.Infrastructure.Data.SqlServer.Tests.Integration;

public class InitializationTests
{
    [Fact]
    public async Task Handle_ValidRequest_InitializesDatabaseCorrectly()
    {
        // arrange
        await using var database = new EmptyDatabaseSqlServerBasedOnConfig();
        await database.InitializeAsync();
        await using var host = new TestHost();
        void registerServices(IServiceCollection services, IConfiguration config)
        {
            services.AddSqlServerInfrastructure(config: config, connectionString: database.GetConnectionString());
            services.Configure<InitializeApplicationDatabaseOptions>(options => ConfigureOptions(options, databaseName: database.DatabaseName));
        }
        await host.InitializeAsync(new TestHostInitializationSettings { RegisterServices = registerServices });
        var mediator = host.ServiceProvider.GetRequiredService<IMediator>();
        var request = new InitializeApplicationDatabaseRequest
        {
            ConnectionString = database.GetConnectionString(),
            DatabaseName = database.DatabaseName,
        };

        // act
        await mediator.Send(request: request, cancellationToken: TestContext.Current.CancellationToken);

        // assert
        await CreateTestTable(connectionString: database.GetConnectionString()); // create a test table in the desired schema that dml operations can be asserted against
        foreach (var user in GetTestDatabaseUsers())
        {
            var connectionString = new SqlConnectionStringBuilder(database.GetConnectionString()) { UserID = user.Name, Password = user.Password, IntegratedSecurity = false }.ConnectionString;
            using var connection = new SqlConnection(connectionString);
            Func<Task> connectionOpenAsync = async () => await connection.OpenAsync();
            await connectionOpenAsync.Should().NotThrowAsync(because: $"this database user with contained database authentication should have been created during initialization: {user.Name}");
            Func<Task> executeSelect = async () => await ExecuteSelect(connection);
            Func<Task> executeInsert = async () => await ExecuteInsert(connection);
            Func<Task> executeUpdate = async () => await ExecuteUpdate(connection);
            Func<Task> executeDelete = async () => await ExecuteDelete(connection);
            if (user.CanSelect)
            {
                await executeSelect.Should().NotThrowAsync(because: "this connection should have permission: select");
            }
            else
            {
                await executeSelect.Should().ThrowAsync<SqlException>(because: "this connection should not have permission: select");
            }
            if (user.CanInsert)
            {
                await executeInsert.Should().NotThrowAsync(because: "this connection should have permission: insert");
            }
            else
            {
                await executeInsert.Should().ThrowAsync<SqlException>(because: "this connection should not have permission: insert");
            }
            if (user.CanUpdate)
            {
                await executeUpdate.Should().NotThrowAsync(because: "this connection should have permission: update");
            }
            else
            {
                await executeUpdate.Should().ThrowAsync<SqlException>(because: "this connection should not have permission: update");
            }
            if (user.CanDelete)
            {
                await executeDelete.Should().NotThrowAsync(because: "this connection should have permission: delete");
            }
            else
            {
                await executeDelete.Should().ThrowAsync<SqlException>(because: "this connection should not have permission: delete");
            }
        }
    }

    private const string Schema = "app";
    private const string TableName = $"[{Schema}].[Test]";
    private const string UserAllDml = nameof(UserAllDml);
    private const string UserAllDmlPassword = "IntegrationTesting!1";
    private const string UserSelectOnly = nameof(UserSelectOnly);
    private const string UserSelectOnlyPassword = "IntegrationTesting!2";
    private const string UserInsertOnly = nameof(UserInsertOnly);
    private const string UserInsertOnlyPassword = "IntegrationTesting!3";
    private const string UserUpdateOnly = nameof(UserUpdateOnly);
    private const string UserUpdateOnlyPassword = "IntegrationTesting!4";
    private const string UserDeleteOnly = nameof(UserDeleteOnly);
    private const string UserDeleteOnlyPassword = "IntegrationTesting!5";

    private class TestDatabaseUser
    {
        public required string Name { get; set; }
        public required string Password { get; set; }
        public bool CanSelect { get; set; }
        public bool CanInsert { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }

    private static IEnumerable<TestDatabaseUser> GetTestDatabaseUsers()
    {
        yield return new TestDatabaseUser { Name = UserAllDml, Password = UserAllDmlPassword, CanSelect = true, CanInsert = true, CanUpdate = true, CanDelete = true };
        yield return new TestDatabaseUser { Name = UserSelectOnly, Password = UserSelectOnlyPassword, CanSelect = true, CanInsert = false, CanUpdate = false, CanDelete = false };
        yield return new TestDatabaseUser { Name = UserInsertOnly, Password = UserInsertOnlyPassword, CanSelect = false, CanInsert = true, CanUpdate = false, CanDelete = false };
        yield return new TestDatabaseUser { Name = UserUpdateOnly, Password = UserUpdateOnlyPassword, CanSelect = false, CanInsert = false, CanUpdate = true, CanDelete = false };
        yield return new TestDatabaseUser { Name = UserDeleteOnly, Password = UserDeleteOnlyPassword, CanSelect = false, CanInsert = false, CanUpdate = false, CanDelete = true };
    }

    private static async Task CreateTestTable(string connectionString)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        using var command = connection.CreateCommand();
        command.CommandText = $"CREATE TABLE {TableName} ([Id] NVARCHAR(1));";
        await command.ExecuteNonQueryAsync();
    }

    private static async Task ExecuteSelect(SqlConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT 1 FROM {TableName} WHERE 0 = 1";
        await command.ExecuteScalarAsync();
    }

    private static async Task ExecuteInsert(SqlConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = $"INSERT INTO {TableName} ([Id]) VALUES ('0');";
        await command.ExecuteNonQueryAsync();
    }

    private static async Task ExecuteUpdate(SqlConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = $"UPDATE {TableName} SET [Id] = 0 WHERE 0 = 1;";
        await command.ExecuteNonQueryAsync();
    }

    private static async Task ExecuteDelete(SqlConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = $"DELETE FROM {TableName} WHERE 0 = 1;";
        await command.ExecuteNonQueryAsync();
    }

    private static void ConfigureOptions(InitializeApplicationDatabaseOptions options, string databaseName)
    {
        options.Contained = true;
        options.Enabled = true;
        options.Name = databaseName;
        options.Roles =
        [
            new InitializeApplicationDatabaseRoleOptions
            {
                Name = "rl_all_dml",
                Grants =
                [
                    new InitializeApplicationDatabaseRoleGrantOptions
                    {
                        Schema = Schema,
                        Permissions = ["SELECT","INSERT","UPDATE","DELETE"],
                    },
                ],
            },
            new InitializeApplicationDatabaseRoleOptions
            {
                Name = "rl_select",
                Grants =
                [
                    new InitializeApplicationDatabaseRoleGrantOptions
                    {
                        Schema = Schema,
                        Permissions = ["SELECT"],
                    },
                ],
            },
            new InitializeApplicationDatabaseRoleOptions
            {
                Name = "rl_insert",
                Grants =
                [
                    new InitializeApplicationDatabaseRoleGrantOptions
                    {
                        Schema = Schema,
                        Permissions = ["INSERT"],
                    },
                ],
            },
            new InitializeApplicationDatabaseRoleOptions
            {
                Name = "rl_update",
                Grants =
                [
                    new InitializeApplicationDatabaseRoleGrantOptions
                    {
                        Schema = Schema,
                        Permissions = ["UPDATE"],
                    },
                ],
            },
            new InitializeApplicationDatabaseRoleOptions
            {
                Name = "rl_delete",
                Grants =
                [
                    new InitializeApplicationDatabaseRoleGrantOptions
                    {
                        Schema = Schema,
                        Permissions = ["DELETE"],
                    },
                ],
            },
        ];
        options.Schemas =
        [
            new InitializeApplicationDatabaseSchemaOptions { Name = Schema },
        ];
        options.SkipIfDbExists = false;
        options.Users = new InitializeApplicationDatabaseUsersOptions
        {
            {
                UserAllDml,
                new InitializeApplicationDatabaseUserOptions
                {
                    Name = UserAllDml,
                    Password = UserAllDmlPassword,
                    LoginName = UserAllDml,
                    Roles = ["rl_all_dml"],
                }
            },
            {
                UserSelectOnly,
                new InitializeApplicationDatabaseUserOptions
                {
                    Name = UserSelectOnly,
                    Password = UserSelectOnlyPassword,
                    LoginName = UserSelectOnly,
                    Roles = ["rl_select"],
                }
            },
            {
                UserInsertOnly,
                new InitializeApplicationDatabaseUserOptions
                {
                    Name = UserInsertOnly,
                    Password = UserInsertOnlyPassword,
                    LoginName = UserInsertOnly,
                    Roles = ["rl_insert"],
                }
            },
            {
                UserUpdateOnly,
                new InitializeApplicationDatabaseUserOptions
                {
                    Name = UserUpdateOnly,
                    Password = UserUpdateOnlyPassword,
                    LoginName = UserUpdateOnly,
                    Roles = ["rl_update"],
                }
            },
            {
                UserDeleteOnly,
                new InitializeApplicationDatabaseUserOptions
                {
                    Name = UserDeleteOnly,
                    Password = UserDeleteOnlyPassword,
                    LoginName = UserDeleteOnly,
                    Roles = ["rl_delete"],
                }
            },
        };
    }
}
