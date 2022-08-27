using System.ComponentModel.DataAnnotations;
using Social_Setting.Comment.Data;
using Social_Setting.User.Data;
using Social_Setting.Vote.Model;

namespace Social_Setting.Vote.Data;

public class CommentVoteEntity
{
    [Key]
    public Guid Id { get; set; }
    
    public CommentEntity Comment { get; set; }

    public UserEntity User { get; set; }

    public VoteDirection VoteDirection { get; set; }
}