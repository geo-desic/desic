using System.ComponentModel.DataAnnotations;

namespace Desic.Api.Dtos.Users;

public class UserCreate
{
    [Required]
    public required string Username { get; set; } = string.Empty;
}
