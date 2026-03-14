using System.ComponentModel.DataAnnotations;

namespace Desic.Application.Users.Create;

public class CreateUser
{
    [Required]
    public required string Username { get; set; }
}
