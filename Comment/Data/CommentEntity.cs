using System.ComponentModel.DataAnnotations;
using Social_Setting.Post.Data;
using Social_Setting.User.Data;
using Social_Setting.Vote.Data;

namespace Social_Setting.Comment.Data;

public class CommentEntity
{
    [Key]
    public Guid Id { get; set; }
    public string Body { get; set; }
    public PostEntity Post { get; set; }
    public UserEntity User { get; set; }
    public ISet<CommentVoteEntity> Votes { get; set; } = new HashSet<CommentVoteEntity>();
    public DateTime CreationDate { get; set; }
}