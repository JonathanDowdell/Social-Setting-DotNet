using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Social_Setting.User.Data;

[Index(nameof(Email), IsUnique = true), 
 Index(nameof(Username), IsUnique = true)]
public class UserEntity
{
    public UserEntity()
    {
    }

    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public string Email { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    public byte[] PasswordHash { get; set; }

    [Required]
    public byte[] PasswordSalt { get; set; }
    
    [Required]
    public DateTime CreationDate { get; set; }

    [Required] public ISet<RoleEntity> Roles { get; set; }

}