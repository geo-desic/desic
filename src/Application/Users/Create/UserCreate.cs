using System.ComponentModel.DataAnnotations;

namespace Desic.Application.Users.Create;

public class UserCreate
{
    [Required]
    public required string Username { get; set; }
}
