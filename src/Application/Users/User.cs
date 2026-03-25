using Desic.Application.Common.Models;
using Desic.Domain.Common.Entities;

namespace Desic.Application.Users;

public class User : SoftDeletableModel
{
    public User() : base() { }
    public User(SoftDeletableEntity entity) : base(entity) { }
    public required string Username { get; set; }
}
