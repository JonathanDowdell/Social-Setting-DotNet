using Social_Setting.Post.Data;
using Social_Setting.Setting.Model;
using Social_Setting.User.Model;

namespace Social_Setting.Post.Model;

public class PostResponse
{
    public PostResponse(PostEntity postEntity)
    {
        this.Id = postEntity.Id;
        this.Title = postEntity.Title;
        this.Body = postEntity.Body;
        this.VoteCount = postEntity.Votes.Sum(vote => (int)vote.VoteDirection);
        this.CommentCount = postEntity.Comments.Count;
        this.CreationDate = postEntity.CreationDate;
        this.User = new UserResponse(postEntity.User);
        this.Setting = new SettingResponse(postEntity.Setting);
    }

    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public int VoteCount { get; }
    public int CommentCount { get;}
    public DateTime CreationDate { get; set; }
    public UserResponse User { get; set; }
    public SettingResponse Setting { get; set; }
}