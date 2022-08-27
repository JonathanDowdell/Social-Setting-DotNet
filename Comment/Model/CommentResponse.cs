using Social_Setting.Comment.Data;

namespace Social_Setting.Comment.Model;

public class CommentResponse
{
    public CommentResponse(CommentEntity commentEntity)
    {
        this.Id = commentEntity.Id;
        this.Body = commentEntity.Body;
        this.Username = commentEntity.User.Username;
        this.VoteCount = commentEntity.Votes.Sum(votes => (int)votes.VoteDirection);
        this.CreationDate = commentEntity.CreationDate;
    }

    public Guid Id { get; set; }
    public string Body { get; set; }
    public string Username { get; set; }
    public int VoteCount { get; set; }
    public DateTime CreationDate { get; set; }
}