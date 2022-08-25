using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Setting.Comment.Model;
using Social_Setting.Comment.Service;
using Social_Setting.User.Service;

namespace Social_Setting.Comment.Controller;

[ApiController, Authorize]
[Route("setting/{settingId}/post/{postId}/comment"), Authorize]
public class CommentController: Microsoft.AspNetCore.Mvc.Controller
{

    private readonly ICommentService _commentService;

    private readonly IUserService _userService;

    public CommentController(ICommentService commentService, IUserService userService)
    {
        _commentService = commentService;
        _userService = userService;
    }

    /// <summary> The GetCommentsForPost function returns a list of comments for the given postId.</summary>
    ///
    /// <param name="postId"> The id of the post to comment on.</param>
    ///
    /// <returns> A list of commentresponse objects</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentResponse>>> GetCommentsForPost(string postId)
    {
        var comments = await _commentService.GetCommentsForPostAsync(postId);
        var commentResponses = comments.Select(comment => new CommentResponse(comment));
        return Ok(commentResponses);
    }

    /// <summary> The CreateCommentForPost function creates a comment for the post with the given id.</summary>
    ///
    /// <param name="postId"> The id of the post to get comments for</param>
    /// <param name="commentRequest"> /// the createcommentrequest class is used to create a comment for a post.</param>
    ///
    /// <returns> It returns a 201 Created response if successful containing the commentresponse object, and 400 Bad Request if not.</returns>
    [HttpPost]
    public async Task<ActionResult<CommentResponse>> CreateCommentForPost(string postId, CreateCommentRequest commentRequest)
    {
        var currentUser = await _userService.GetCurrentUserAsync();
        var commentEntity = await _commentService.CreateCommentAsync(postId, commentRequest, currentUser);
        return Created("/comment", new CommentResponse(commentEntity));
    }
    
}