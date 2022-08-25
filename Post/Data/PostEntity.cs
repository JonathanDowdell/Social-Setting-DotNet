using Social_Setting.Comment.Data;
using Social_Setting.Setting.Data;
using Social_Setting.User.Data;
using Social_Setting.Vote.Data;

namespace Social_Setting.Post.Data;

public class PostEntity
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string Body { get; set; }

    public DateTime CreationDate { get; set; }

    public ISet<VoteEntity> Votes { get; } = new HashSet<VoteEntity>();

    public List<CommentEntity> Comments { get; set; } = new();

    public SettingEntity Setting { get; set; }

    public UserEntity User { get; set; }
}