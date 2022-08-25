using System.Net;
using Microsoft.EntityFrameworkCore;
using Social_Setting.Comment.Data;
using Social_Setting.Database;
using Social_Setting.Exception;
using Social_Setting.Post.Data;
using Social_Setting.Post.Model;
using Social_Setting.User.Data;
using Social_Setting.Vote.Data;

namespace Social_Setting.Post.Service;

public class PostService : IPostService
{
    
    private readonly ApplicationApiDbContext _apiDbContext;

    public PostService(ApplicationApiDbContext apiDbContext)
    {
        _apiDbContext = apiDbContext;
    }

    /// <summary> The CreatePostForSettingAsync function creates a post for the setting with the given id.</summary>
    /// 
    /// 
    /// <param name="userEntity"> The user creating the post.</param>
    /// <param name="id"> The id of the post to be deleted.</param>
    /// <param name="createPostRequest">Object containing the title and body of the post.</param>
    /// 
    /// <returns> A task that contains the postentity object.</returns>
    public async Task<PostEntity> CreatePostForSettingAsync(UserEntity userEntity, string id, CreatePostRequest createPostRequest)
    {
        try
        {
            var guid = Guid.Parse(id);
            var settingEntity = await _apiDbContext.Settings.FirstOrDefaultAsync(setting => setting.Id.Equals(guid));
            if (settingEntity == null)
            {
                throw new SocialSettingException(HttpStatusCode.NotFound, "Setting Not Found.");
            }

            var unsavedPost = new PostEntity()
            {
                Id = Guid.NewGuid(),
                Title = createPostRequest.Title,
                Body = createPostRequest.Body,
                // Votes = new HashSet<VoteEntity>(),
                Comments = new List<CommentEntity>(),
                CreationDate = DateTime.Now,
                Setting = settingEntity,
                User = userEntity
            };

            await _apiDbContext.AddAsync(unsavedPost);
            await _apiDbContext.SaveChangesAsync();
            return unsavedPost;
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

    /// <summary> The GetPostFromSettingAsync function retrieves a post from the database based on its ID.</summary>
    ///
    /// <param name="postId"> The post id.</param>
    ///
    /// <returns> A postentity object.</returns>
    public async Task<PostEntity> GetPostFromSettingAsync(string postId)
    {
        try
        {
            var guid = Guid.Parse(postId);
            var postEntity = await _apiDbContext.Posts
                .Include(post => post.Setting)
                .Include(post => post.User)
                .Include(post => post.Votes)
                .Include(post => post.Comments)
                .FirstOrDefaultAsync(post => post.Id.Equals(guid));
            if (postEntity == null) throw new SocialSettingException(HttpStatusCode.NotFound, "Post Not Found");
            return postEntity;
        }
        catch (SocialSettingException)
        {
            throw;
        }
        catch (System.Exception e)
        {
            throw new SocialSettingException(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    /// <summary> The GetPostsFromSettingAsync function returns a list of posts from the database that are associated with a specific setting.</summary>
    ///
    /// <param name="id"> The id of the post</param>
    ///
    /// <returns> A list of posts from a specific setting.</returns>
    public async Task<IEnumerable<PostEntity>> GetPostsFromSettingAsync(string id)
    {
        var guid = Guid.Parse(id);
        var posts = await _apiDbContext.Posts
            .Include(post => post.Setting)
            .Include(post => post.User)
            .Include(post => post.Votes)
            .Include(post => post.Comments)
            .Include(post => post.User).ThenInclude(user => user.Roles)
            .Where(post => post.Setting.Id.Equals(guid))
            .OrderByDescending(post => post.CreationDate)
            .ToListAsync();
        return posts;
    }

    /// <summary> The DeletePostFromSettingAsync function deletes a post from the database.</summary>
    ///
    /// <param name="id"> The unique identifier of the post</param>
    ///
    /// <returns> A postentity object</returns>
    public async Task<PostEntity> DeletePostFromSettingAsync(string id)
    {
        try
        {
            var guid = Guid.Parse(id);
            var postEntity = await _apiDbContext.Posts
                .Include(post => post.User)
                .Include(post => post.Setting)
                .FirstOrDefaultAsync(post => post.Id.Equals(guid));
            if (postEntity == null) throw new SocialSettingException(HttpStatusCode.NotFound, "Post Not Found");
            _apiDbContext.Posts.Remove(postEntity);
            await _apiDbContext.SaveChangesAsync();
            return postEntity;
        }
        catch (SocialSettingException)
        {
            throw;
        }
        catch (System.Exception e)
        {
            throw new SocialSettingException(HttpStatusCode.InternalServerError, e.Message);
        }
    }

}