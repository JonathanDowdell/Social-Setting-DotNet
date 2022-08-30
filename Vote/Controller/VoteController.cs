using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Setting.User.Service;
using Social_Setting.Vote.Service;

namespace Social_Setting.Vote.Controller;

[ApiController]
[Route("setting/{settingId}/post/{postId}"), Authorize]
public class VoteController: Microsoft.AspNetCore.Mvc.Controller
{
    private readonly IVoteService _voteService;

    private readonly IUserService _userService;

    public VoteController(IVoteService voteService, IUserService userService)
    {
        _voteService = voteService;
        _userService = userService;
    }

    [HttpPost("vote/up")]
    public async Task UpVotePost(string postId)
    {
        var currentUser = await _userService.GetCurrentUserAsync();
        await _voteService.UpVotePostAsync(postId, currentUser);
    }
    
    [HttpPost("vote/down")]
    public async Task DownVotePost(string postId)
    {
        var currentUser = await _userService.GetCurrentUserAsync();
        await _voteService.DownVotePostAsync(postId, currentUser);
    }

    [HttpDelete("vote/remove")]
    public async Task RemoveVoteFromPost(string postId)
    {
        var currentUser = await _userService.GetCurrentUserAsync();
        await _voteService.RemoveVoteFromPostAsync(postId, currentUser);
    }

    [HttpPost("comment/{commentId}/vote/up")]
    public async Task UpVoteComment(string commentId)
    {
        var currentUser = await _userService.GetCurrentUserAsync();
        await _voteService.UpVoteCommentAsync(commentId, currentUser);
    }
    
    [HttpPost("comment/{commentId}/vote/down")]
    public async Task DownVoteComment(string commentId)
    {
        var currentUser = await _userService.GetCurrentUserAsync();
        await _voteService.DownVoteCommentAsync(commentId, currentUser);
    }

    [HttpDelete("comment/{commentId}/vote/{voteId}/remove")]
    public async Task RemoveVoteFromComment(string voteId)
    {
        var currentUser = await _userService.GetCurrentUserAsync();
        await _voteService.RemoveVoteFromCommentAsync(voteId, currentUser);
    }
}