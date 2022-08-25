using Social_Setting.Comment.Data;
using Social_Setting.Comment.Model;
using Social_Setting.User.Data;

namespace Social_Setting.Comment.Service;

public interface ICommentService
{
    /// <summary> The CreateCommentAsync function creates a comment on a post.</summary>
    /// 
    /// <param name="guidPostString"> The post id.</param>
    /// <param name="commentRequest"> The comment body object.</param>
    /// <param name="currentUser"> The user who is trying to create the comment.</param>
    /// <returns> A task of the commententity.</returns>
    public Task<CommentEntity> CreateCommentAsync(string guidPostString, CreateCommentRequest commentRequest, UserEntity currentUser);
    
    /// <summary> The GetCommentsForPostAsync function retrieves all comments for a given post.</summary>
    ///
    /// <param name="postId"> The post id</param>
    ///
    /// <returns> An ienumerable of commententity objects</returns>
    public Task<IEnumerable<CommentEntity>> GetCommentsForPostAsync(string postId);
}