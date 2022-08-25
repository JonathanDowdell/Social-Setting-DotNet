using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Setting.User.Service;
using Social_Setting.Vote.Service;

namespace Social_Setting.Vote.Controller;

[ApiController]
[Route("setting/{settingId}/post/{postId}/vote"), Authorize]
public class VoteController: Microsoft.AspNetCore.Mvc.Controller
{
    private readonly IVoteService _voteService;

    private readonly IUserService _userService;

    public VoteController(IVoteService voteService, IUserService userService)
    {
        _voteService = voteService;
        _userService = userService;
    }

    [HttpPost("up")]
    public async Task UpVotePost(string postId)
    {
        var currentUser = await _userService.GetCurrentUserAsync();
        await _voteService.UpVotePostAsync(postId, currentUser);
    }
    
    [HttpPost("down")]
    public async Task DownVotePost(string postId)
    {
        var currentUser = await _userService.GetCurrentUserAsync();
        await _voteService.DownVotePostAsync(postId, currentUser);
    }

    [HttpDelete("remove")]
    public async Task RemoveVoteFromPost(string postId)
    {
        var currentUser = await _userService.GetCurrentUserAsync();
        await _voteService.RemoveVoteFromPostAsync(postId, currentUser);
    }

    [HttpPost("{commentId}/up")]
    public async Task UpVoteComment(string commentId)
    {
        var currentUser = await _userService.GetCurrentUserAsync();
    }
}