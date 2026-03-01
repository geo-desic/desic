using Desic.Application.Common.Interfaces;
using Desic.Application.Common.Models;

namespace Desic.Application.Users;

public class User : ICreated, IModified, ISoftDeleted
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public RequiredOnByType Created { get; set; } = new();
    public RequiredOnByType Modified { get; set; } = new();
    public OptionalOnByType Deleted { get; set; } = new();
}
