using Social_Setting.User.Data;

namespace Social_Setting.Vote.Service;

public interface IVoteService
{
    /// <summary> The UpVotePostAsync function is used to upvote a post.</summary>
    ///
    /// <param name="postId"> The post to down vote</param>
    /// <param name="currentUser"> The user who is voting</param>
    ///
    /// <returns> A task.</returns>
    public Task UpVotePostAsync(string postId, UserEntity currentUser);
    
    /// <summary> The DownVotePostAsync function downvotes a post.</summary>
    ///
    /// <param name="postId"> The post to upvote.</param>
    /// <param name="currentUser"> The user that is voting</param>
    ///
    /// <returns> A task.</returns>
    public Task DownVotePostAsync(string postId, UserEntity currentUser);
    
    /// <summary> The RemoveVoteFromPostAsync function removes a vote from the PostVotes table.</summary>
    ///
    /// <param name="postId"> The post id.</param>
    /// <param name="currentUser"> </param>
    ///
    /// <returns> A task that represents the asynchronous operation.</returns>
    public Task RemoveVoteFromPostAsync(string postId, UserEntity currentUser);
    
    /// <summary> The UpVoteCommentAsync function upvotes a comment.</summary>
    ///
    /// <param name="commentId"> The comment to downvote</param>
    /// <param name="currentUser"> The user who is voting</param>
    ///
    /// <returns> A task.</returns>
    public Task UpVoteCommentAsync(string commentId, UserEntity currentUser);
    
    /// <summary> The RemoveVoteFromCommentAsync function removes a vote from the comment with the given id.</summary>
    ///
    /// <param name="commentId"> The id of the comment to be updated</param>
    /// <param name="currentUser"> The user who is voting</param>
    ///
    /// <returns> A task.</returns>
    public Task DownVoteCommentAsync(string commentId, UserEntity currentUser);
    
    /// <summary> The RemoveVoteFromCommentAsync function removes a vote from the comment with the given id.</summary>
    ///
    /// <param name="voteId"> The id of the comment to be updated</param>
    /// <param name="currentUser"> The user who is voting</param>
    ///
    /// <returns> A task.</returns>
    public Task RemoveVoteFromCommentAsync(string voteId, UserEntity currentUser);
}