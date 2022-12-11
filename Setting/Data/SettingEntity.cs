using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Social_Setting.Post.Data;
using Social_Setting.User.Data;

namespace Social_Setting.Setting.Data;

[Index(nameof(Title), IsUnique = true)]
public class SettingEntity
{
    [Key]
    public Guid Id { get; set; }
    
    public string PhotoURL { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public DateTime CreationDate { get; set; }
    
    public UserEntity User { get; set; }
    
    public List<PostEntity> Post { get; set; }
}