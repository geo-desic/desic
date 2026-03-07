using System.ComponentModel.DataAnnotations;

namespace Desic.Infrastructure.Data.SqlServer;

public sealed class DatabaseInitializerOptions
{
    [Required]
    [StringLength(123, MinimumLength = 1)]
    [RegularExpression("^[a-zA-Z_]{1}[a-zA-Z0-9_\\@\\#\\$]*$")] // start with alphabetic character or '_'; remaining characters can be alphanumeric characters or: '_', '@', '#', '$'
    public string? Name { get; set; }
    public bool? Contained { get; set; }
    public bool? NoInitIfDbExists { get; set; }
    public List<DatabaseInitializerRoleOptions>? Roles { get; set; }
    public List<DatabaseInitializerSchemaOptions>? Schemas { get; set; }
    public Dictionary<string, DatabaseInitializerUserOptions>? Users { get; set; }
}

public sealed class DatabaseInitializerRoleOptions
{
    [Required]
    [StringLength(128, MinimumLength = 1)]
    [RegularExpression("^[a-zA-Z_]{1}[a-zA-Z0-9_\\@\\#\\$]*$")] // start with alphabetic character or: '_'; remaining characters can be alphanumeric characters or: '_', '@', '#', '$'
    public string? Name { get; set; }
    [StringLength(128, MinimumLength = 1)]
    [RegularExpression("^[a-zA-Z_]{1}[a-zA-Z0-9_\\@\\#\\$]*$")] // start with alphabetic character or: '_'; remaining characters can be alphanumeric characters or: '_', '@', '#', '$'
    public string? OwnerName { get; set; }
    public List<DatabaseInitializerRoleGrantOptions>? Grants { get; set; }
}

public sealed class DatabaseInitializerRoleGrantOptions
{
    public List<string?>? Permissions { get; set; }
    [Required]
    [StringLength(128, MinimumLength = 1)]
    [RegularExpression("^[a-zA-Z_]{1}[a-zA-Z0-9_\\@\\#\\$]*$")] // start with alphabetic character or: '_'; remaining characters can be alphanumeric characters or: '_', '@', '#', '$'
    public string? Schema { get; set; }
}

public sealed class DatabaseInitializerRoleSchemaGrantPermissionOptions
{
    [Required]
    [RegularExpression("^ALTER|CONTROL|CREATE SEQUENCE|DELETE|EXECUTE|INSERT|REFERENCES|SELECT|TAKE OWNERSHIP|UPDATE|VIEW DEFINITION$")]
    public string? Name { get; set; }
}

public sealed class DatabaseInitializerSchemaOptions
{
    [Required]
    [StringLength(128, MinimumLength = 1)]
    [RegularExpression("^[a-zA-Z_]{1}[a-zA-Z0-9_\\@\\#\\$]*$")] // start with alphabetic character or: '_'; remaining characters can be alphanumeric characters or: '_', '@', '#', '$'
    public string? Name { get; set; }
    [StringLength(128, MinimumLength = 1)]
    [RegularExpression("^[a-zA-Z_]{1}[a-zA-Z0-9_\\@\\#\\$]*$")] // start with alphabetic character or: '_'; remaining characters can be alphanumeric characters or: '_', '@', '#', '$'
    public string? OwnerName { get; set; }
}

public sealed class DatabaseInitializerUserOptions
{
    [Required]
    [StringLength(128, MinimumLength = 1)]
    [RegularExpression("^[a-zA-Z_\\@\\#]{1}[a-zA-Z0-9_\\@\\#\\$]*$")] // start with alphabetic character or: '_', '@', '#'; remaining characters can be alphanumeric characters or: '_', '@', '#', '$'
    public string? Name { get; set; }
    [Required]
    [StringLength(128, MinimumLength = 8)]
    [RegularExpression("^(?=(.*\\d){1,})(?=(.*[a-z]){1,})(?=(.*[A-Z]){1,})(?=(.*[^\\w\\d\\s]){1,})", ErrorMessage = "{0} does not satisfy complexity rules")] // contains at least 3 of the following: (1) a digit (2) a lowercase letter (3) an uppercase letter (4) special character (i.e. non-alphanumeric)
    public string? Password { get; set; }
    [StringLength(128, MinimumLength = 1)]
    [RegularExpression("^[a-zA-Z_\\@\\#]{1}[a-zA-Z0-9_\\@\\#\\$]*$")] // start with alphabetic character or: '_', '@', '#'; remaining characters can be alphanumeric characters or: '_', '@', '#', '$'
    public string? LoginName { get; set; }
    [StringLength(128, MinimumLength = 1)]
    [RegularExpression("^[a-zA-Z_]{1}[a-zA-Z0-9_\\@\\#\\$]*$")] // start with alphabetic character or: '_'; remaining characters can be alphanumeric characters or: '_', '@', '#', '$'
    public string? DefaultSchema { get; set; }
    public List<string?>? Roles { get; set; }
}