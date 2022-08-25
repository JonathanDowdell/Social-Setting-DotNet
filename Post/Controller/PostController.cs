using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Setting.Post.Model;
using Social_Setting.Post.Service;
using Social_Setting.User.Service;

namespace Social_Setting.Post.Controller;

[ApiController]
[Route("setting/{settingId}/post"), Authorize]
public class PostController : Microsoft.AspNetCore.Mvc.Controller
{

    private readonly IPostService _postService;

    private readonly IUserService _userService;

    public PostController(IPostService postService, IUserService userService)
    {
        _postService = postService;
        _userService = userService;
    }
    

    /// <summary> The GetPostsFromSetting function returns a list of posts from the specified setting.</summary>
    ///
    /// <param name="settingId"> The id of the setting from which to get posts</param>
    ///
    /// <returns> A list of postresponse objects.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostResponse>>> GetPostsFromSetting(string settingId)
    {
        var postEntities = await _postService.GetPostsFromSettingAsync(settingId);
        var postResponses = postEntities.Select(post => new PostResponse(post)).ToList();
        return Ok(postResponses);
    }

    /// <summary> The GetPostFromSetting function returns a post from the database based on the postId.</summary>
    ///
    /// <param name="postId"> The id of the post to be retrieved</param>
    ///
    /// <returns> A postresponse object.</returns>
    [HttpGet("{postId}")]
    public async Task<ActionResult<PostResponse>> GetPostFromSetting(string postId)
    {
        var postEntity = await _postService.GetPostFromSettingAsync(postId);
        return Ok(new PostResponse(postEntity));
    }

    /// <summary> The CreatePostForSetting function creates a post for the setting with the given id.</summary>
    ///
    /// <param name="settingId"> The id of the setting</param>
    /// <param name="createPostRequest"> An Object containing the Title and Body of the post to be created</param>
    ///
    /// <returns> A postresponse object. This object contains the newly created post and its associated user.</returns>
    [HttpPost]
    public async Task<ActionResult<PostResponse>> CreatePostForSetting(string settingId, CreatePostRequest createPostRequest)
    {
        var currentUser = await _userService.GetCurrentUserAsync();
        var createdPost = await _postService.CreatePostForSettingAsync(currentUser, settingId, createPostRequest);
        return Ok(new PostResponse(createdPost));
    }

    /// <summary> The DeletePostFromSetting function deletes a post from the user's settings.</summary>
    ///
    /// <param name="postId"> The postId of the post to be updated</param>
    ///
    /// <returns> A postresponse object.</returns>
    [HttpDelete("{postId}")]
    public async Task<ActionResult<PostResponse>> DeletePostFromSetting(string postId)
    {
        var deletedPostEntity = await _postService.DeletePostFromSettingAsync(postId);
        return Ok(new PostResponse(deletedPostEntity));
    }
}