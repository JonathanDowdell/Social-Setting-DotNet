using Social_Setting.User.Data;

namespace Social_Setting.Vote.Service;

public interface IVoteService
{
    public Task UpVotePostAsync(string postId, UserEntity currentUser);
    public Task DownVotePostAsync(string postId, UserEntity currentUser);
    public Task RemoveVoteFromPostAsync(string postId, UserEntity currentUser);
    public Task UpVoteCommentAsync(string commentId, UserEntity currentUser);
    public Task DownVoteCommentAsync(string commentId, UserEntity currentUser);
    public Task RemoveVoteFromCommentAsync(string commentId, UserEntity currentUser);
}