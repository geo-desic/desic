using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace Desic.Infrastructure.Data.SqlServer;

public sealed class InitializeApplicationDatabaseOptions
{
    public bool? Contained { get; set; }
    public bool? Enabled { get; set; }
    [Required]
    [StringLength(123, MinimumLength = 1)]
    [RegularExpression("^[a-zA-Z_]{1}[a-zA-Z0-9_\\@\\#\\$]*$")] // start with alphabetic character or '_'; remaining characters can be alphanumeric characters or: '_', '@', '#', '$'
    public string? Name { get; set; }
    [ValidateEnumeratedItems]
    public List<InitializeApplicationDatabaseRoleOptions>? Roles { get; set; }
    [ValidateEnumeratedItems]
    public List<InitializeApplicationDatabaseSchemaOptions>? Schemas { get; set; }
    public bool? SkipIfDbExists { get; set; }
    public InitializeApplicationDatabaseUsersOptions? Users { get; set; }
}

public sealed class InitializeApplicationDatabaseRoleOptions
{
    [Required]
    [StringLength(128, MinimumLength = 1)]
    [RegularExpression("^[a-zA-Z_]{1}[a-zA-Z0-9_\\@\\#\\$]*$")] // start with alphabetic character or: '_'; remaining characters can be alphanumeric characters or: '_', '@', '#', '$'
    public string? Name { get; set; }
    [StringLength(128, MinimumLength = 1)]
    [RegularExpression("^[a-zA-Z_]{1}[a-zA-Z0-9_\\@\\#\\$]*$")] // start with alphabetic character or: '_'; remaining characters can be alphanumeric characters or: '_', '@', '#', '$'
    public string? OwnerName { get; set; }
    [ValidateEnumeratedItems]
    public List<InitializeApplicationDatabaseRoleGrantOptions>? Grants { get; set; }
}

public sealed class InitializeApplicationDatabaseRoleGrantOptions
{
    public List<string?>? Permissions { get; set; }
    [Required]
    [StringLength(128, MinimumLength = 1)]
    [RegularExpression("^[a-zA-Z_]{1}[a-zA-Z0-9_\\@\\#\\$]*$")] // start with alphabetic character or: '_'; remaining characters can be alphanumeric characters or: '_', '@', '#', '$'
    public string? Schema { get; set; }
}

public sealed class InitializeApplicationDatabaseRoleSchemaGrantPermissionOptions
{
    [Required]
    [RegularExpression("^ALTER|CONTROL|CREATE SEQUENCE|DELETE|EXECUTE|INSERT|REFERENCES|SELECT|TAKE OWNERSHIP|UPDATE|VIEW DEFINITION$")]
    public string? Name { get; set; }
}

public sealed class InitializeApplicationDatabaseSchemaOptions
{
    [Required]
    [StringLength(128, MinimumLength = 1)]
    [RegularExpression("^[a-zA-Z_]{1}[a-zA-Z0-9_\\@\\#\\$]*$")] // start with alphabetic character or: '_'; remaining characters can be alphanumeric characters or: '_', '@', '#', '$'
    public string? Name { get; set; }
    [StringLength(128, MinimumLength = 1)]
    [RegularExpression("^[a-zA-Z_]{1}[a-zA-Z0-9_\\@\\#\\$]*$")] // start with alphabetic character or: '_'; remaining characters can be alphanumeric characters or: '_', '@', '#', '$'
    public string? OwnerName { get; set; }
}

public sealed class InitializeApplicationDatabaseUsersOptions : Dictionary<string, InitializeApplicationDatabaseUserOptions>
{
    public InitializeApplicationDatabaseUserOptions? Api
    {
        get
        {
            TryGetValue(nameof(Api), out InitializeApplicationDatabaseUserOptions? value);
            return value;
        }
    }
    public InitializeApplicationDatabaseUserOptions? Migrations
    {
        get
        {
            TryGetValue(nameof(Migrations), out InitializeApplicationDatabaseUserOptions? value);
            return value;
        }
    }
}

public sealed class InitializeApplicationDatabaseUserOptions
{
    [Required]
    [StringLength(128, MinimumLength = 1)]
    [RegularExpression("^[a-zA-Z_\\@\\#]{1}[a-zA-Z0-9_\\@\\#\\$]*$")] // start with alphabetic character or: '_', '@', '#'; remaining characters can be alphanumeric characters or: '_', '@', '#', '$'
    public string? Name { get; set; }
    [StringLength(128, MinimumLength = 8)]
    [RegularExpression("^(?=(.*\\d){1,})(?=(.*[a-z]){1,})(?=(.*[A-Z]){1,})(?=(.*[^\\w\\d\\s]){1,})", ErrorMessage = "{0} does not satisfy complexity rules")] // contains at least 3 of the following: (1) a digit (2) a lowercase letter (3) an uppercase letter (4) special character (i.e. non-alphanumeric)
    public string? Password { get; set; }
    public string? PasswordConfigKey { get; set; }
    [StringLength(128, MinimumLength = 1)]
    [RegularExpression("^[a-zA-Z_\\@\\#]{1}[a-zA-Z0-9_\\@\\#\\$]*$")] // start with alphabetic character or: '_', '@', '#'; remaining characters can be alphanumeric characters or: '_', '@', '#', '$'
    public string? LoginName { get; set; }
    [StringLength(128, MinimumLength = 1)]
    [RegularExpression("^[a-zA-Z_]{1}[a-zA-Z0-9_\\@\\#\\$]*$")] // start with alphabetic character or: '_'; remaining characters can be alphanumeric characters or: '_', '@', '#', '$'
    public string? DefaultSchema { get; set; }
    public List<string?>? Roles { get; set; }
}