using System.ComponentModel.DataAnnotations;
using Social_Setting.Post.Data;
using Social_Setting.User.Data;
using Social_Setting.Vote.Model;

namespace Social_Setting.Vote.Data;

public class VoteEntity
{
    [Key]
    public Guid Id { get; set; }
    
    public PostEntity Post { get; set; }

    public UserEntity User { get; set; }

    public VoteDirection VoteDirection { get; set; }
}