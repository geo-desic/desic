using System.ComponentModel.DataAnnotations;

namespace Desic.Infrastructure.Data.Providers;

public sealed class ConnectionBehaviorOptions
{
    [Required]
    public string? ConnectionStringName { get; set; }
    public bool? RemoveInitialCatalog { get; set; }
    public bool? ReplacePassword { get; set; }
    public bool? ReplaceUserId { get; set; }
    public string? UserId { get; set; }
    public string? Password { get; set; }
}