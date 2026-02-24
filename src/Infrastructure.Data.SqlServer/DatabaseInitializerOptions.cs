namespace Desic.Infrastructure.Data.SqlServer;

public sealed class DatabaseInitializerOptions
{
    public string? Name { get; set; }
    public bool? Contained { get; set; }
    public bool? StopIfExists { get; set; }
    public List<DatabaseInitializerRoleOptions>? Roles { get; set; }
    public List<DatabaseInitializerSchemaOptions>? Schemas { get; set; }
    public List<DatabaseInitializerUserOptions>? Users { get; set; }
}

public sealed class DatabaseInitializerRoleOptions
{
    public string? Name { get; set; }
    public string? OwnerName { get; set; }
    public List<DatabaseInitializerRoleGrantOptions>? Grants { get; set; }
}

public sealed class DatabaseInitializerRoleGrantOptions
{
    public List<string?>? Permissions { get; set; }
    public string? Schema { get; set; }
}

public sealed class DatabaseInitializerRoleSchemaGrantPermissionOptions
{
    public string? Name { get; set; }
}

public sealed class DatabaseInitializerSchemaOptions
{
    public string? Name { get; set; }
    public string? OwnerName { get; set; }
}

public sealed class DatabaseInitializerUserOptions
{
    public string? Name { get; set; }
    public string? Password { get; set; }
    public string? LoginName { get; set; }
    public string? DefaultSchema { get; set; }
    public List<string?>? Roles { get; set; }
}