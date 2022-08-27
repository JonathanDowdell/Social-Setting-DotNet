using System.Net;
using Microsoft.EntityFrameworkCore;
using Social_Setting.Comment.Data;
using Social_Setting.Comment.Model;
using Social_Setting.Database;
using Social_Setting.Exception;
using Social_Setting.User.Data;

namespace Social_Setting.Comment.Service;

public class CommentService : ICommentService
{
    
    private readonly ApplicationApiDbContext _apiDbContext;

    public CommentService(ApplicationApiDbContext apiDbContext)
    {
        _apiDbContext = apiDbContext;
    }

    /// <summary> The CreateCommentAsync function creates a comment on a post.</summary>
    /// 
    /// <param name="guidPostString"> The post id.</param>
    /// <param name="commentRequest"> The comment body object.</param>
    /// <param name="currentUser"> The user who is trying to create the comment.</param>
    /// <returns> A task of the commententity.</returns>
    public async Task<CommentEntity> CreateCommentAsync(string guidPostString, CreateCommentRequest commentRequest, UserEntity currentUser)
    {
        try
        {
            var postId = Guid.Parse(guidPostString);
            var post = await _apiDbContext.Posts
                .FindAsync(postId);

            if (post == null) throw new SocialSettingException(HttpStatusCode.NotFound, "Post Not Found.");

            var unsavedCommentEntity = new CommentEntity()
            {
                Id = Guid.NewGuid(),
                Body = commentRequest.Body,
                User = currentUser,
                Post = post,
                CreationDate = DateTime.Now
            };

            await _apiDbContext.Comments.AddAsync(unsavedCommentEntity);

            await _apiDbContext.SaveChangesAsync();

            return unsavedCommentEntity;
        }
        catch (SocialSettingException)
        {
            throw;
        }
        catch (FormatException)
        {
            throw new SocialSettingException(HttpStatusCode.BadRequest, "ID is invalid");
        }
        catch (System.Exception e)
        {
            throw new SocialSettingException(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    /// <summary> The GetCommentsForPostAsync function retrieves all comments for a given post.</summary>
    ///
    /// <param name="postId"> The post id</param>
    ///
    /// <returns> An ienumerable of commententity objects</returns>
    public async Task<IEnumerable<CommentEntity>> GetCommentsForPostAsync(string postId)
    {
        try
        {
            var id = Guid.Parse(postId);
            var comments = await _apiDbContext.Comments
                .Include(comment => comment.Votes)
                .Include(comment => comment.User)
                .Where(comment => comment.Post.Id.Equals(id))
                .ToArrayAsync();
            return comments;
        }
        catch (SocialSettingException)
        {
            throw;
        }
        catch (FormatException)
        {
            throw new SocialSettingException(HttpStatusCode.BadRequest, "ID is invalid");
        }
        catch (System.Exception e)
        {
            throw new SocialSettingException(HttpStatusCode.InternalServerError, e.Message);
        }
    }
}