using Desic.Application.Common.Models;

namespace Desic.Application.Users;

public class User : SoftDeletableDto
{
    public required string Username { get; set; }
}
