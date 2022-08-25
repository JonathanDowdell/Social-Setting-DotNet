using Social_Setting.Post.Data;
using Social_Setting.Post.Model;
using Social_Setting.User.Data;

namespace Social_Setting.Post.Service;

public interface IPostService
{
    /// <summary> The GetPostFromSettingAsync function retrieves a post from the database based on its ID.</summary>
    ///
    /// <param name="postId"> The post id.</param>
    ///
    /// <returns> A postentity object.</returns>
    public Task<PostEntity> GetPostFromSettingAsync(string postId);
    
    /// <summary> The GetPostsFromSettingAsync function returns a list of posts from the database that are associated with a specific setting.</summary>
    ///
    /// <param name="id"> The id of the post</param>
    ///
    /// <returns> A list of posts from a specific setting.</returns>
    public Task<IEnumerable<PostEntity>> GetPostsFromSettingAsync(string id);
    
    /// <summary> The DeletePostFromSettingAsync function deletes a post from the database.</summary>
    ///
    /// <param name="id"> The unique identifier of the post</param>
    ///
    /// <returns> A postentity object</returns>
    public Task<PostEntity> DeletePostFromSettingAsync(string id);
    
    /// <summary> The CreatePostForSettingAsync function creates a post for the setting with the given id.</summary>
    /// 
    /// 
    /// <param name="userEntity"> The user creating the post.</param>
    /// <param name="id"> The id of the post to be deleted.</param>
    /// <param name="createPostRequest">Object containing the title and body of the post.</param>
    /// 
    /// <returns> A task that contains the postentity object.</returns>
    public Task<PostEntity> CreatePostForSettingAsync(UserEntity userEntity, string id, CreatePostRequest createPostRequest);
}