using System.ComponentModel.DataAnnotations;
using Social_Setting.Post.Data;
using Social_Setting.User.Data;

namespace Social_Setting.Comment.Data;

public class CommentEntity
{
    [Key]
    public Guid Id { get; set; }
    public string Body { get; set; }
    public DateTime CreationDate { get; set; }
    public PostEntity Post { get; set; }
    public UserEntity User { get; set; }
}