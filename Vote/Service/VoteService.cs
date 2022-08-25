using System.Net;
using Microsoft.EntityFrameworkCore;
using Social_Setting.Comment.Data;
using Social_Setting.Database;
using Social_Setting.Exception;
using Social_Setting.Extension;
using Social_Setting.User.Data;
using Social_Setting.Vote.Data;
using Social_Setting.Vote.Model;

namespace Social_Setting.Vote.Service;

public class VoteService : IVoteService
{
    
    private readonly ApplicationApiDbContext _apiDbContext;

    public VoteService(ApplicationApiDbContext apiDbContext)
    {
        _apiDbContext = apiDbContext;
    }

    public async Task UpVotePostAsync(string postId, UserEntity currentUser)
    {
        await VoteForPostAsync(postId, currentUser, VoteDirection.Up);
    }

    public async Task DownVotePostAsync(string postId, UserEntity currentUser)
    {
        await VoteForPostAsync(postId, currentUser, VoteDirection.Down);
    }

    private async Task VoteForPostAsync(string postId, UserEntity currentUser, VoteDirection voteDirection)
    {
        var postGuid = GuidExtensions.ParseOrThrow(postId);

        var existingVoteEntity = await _apiDbContext.Votes
            .FirstOrDefaultAsync(vote => 
                vote.User.Id.Equals(currentUser.Id) && 
                vote.Post.Id.Equals(postGuid) 
                && vote.VoteDirection.Equals(voteDirection));

        if (existingVoteEntity != null)
        {
            throw new SocialSettingException(HttpStatusCode.Conflict, "Vote already exist for post.");
        }

        var post = await _apiDbContext.Posts
            .FindAsync(postGuid);

        if (post == null)
        {
            throw new SocialSettingException(HttpStatusCode.NotFound, "Post Not Found.");
        }

        await RemoveOppositeVoteFromPostAsync(voteDirection, currentUser, postGuid);
        
        var voteEntity = new VoteEntity()
        {
            Id = Guid.NewGuid(),
            Post = post,
            User = currentUser,
            VoteDirection = voteDirection
        };

        _apiDbContext.Votes.Add(voteEntity);
        
        await _apiDbContext.SaveChangesAsync();
        
    }

    private async Task RemoveOppositeVoteFromPostAsync(VoteDirection currentDirection, UserEntity currentUser, Guid postGuid)
    {
        VoteEntity? existingVoteEntity;
        if (currentDirection.Equals(VoteDirection.Down))
        {
            existingVoteEntity = await _apiDbContext.Votes
                .FirstOrDefaultAsync(vote => 
                    vote.User.Id.Equals(currentUser.Id) && 
                    vote.Post.Id.Equals(postGuid) 
                    && vote.VoteDirection.Equals(VoteDirection.Up));
        }
        else
        {
            existingVoteEntity = await _apiDbContext.Votes
                .FirstOrDefaultAsync(vote => 
                    vote.User.Id.Equals(currentUser.Id) && 
                    vote.Post.Id.Equals(postGuid) 
                    && vote.VoteDirection.Equals(VoteDirection.Down));
        }

        if (existingVoteEntity != null)
        {
            _apiDbContext.Votes.Remove(existingVoteEntity);
        }
    }

    public async Task RemoveVoteFromPostAsync(string postId, UserEntity currentUser)
    {
        var postGuid = GuidExtensions.ParseOrThrow(postId);
        
        var existingVoteEntity = await _apiDbContext.Votes
            .FirstOrDefaultAsync(vote => 
                vote.User.Id.Equals(currentUser.Id) && 
                vote.Post.Id.Equals(postGuid));

        if (existingVoteEntity == null)
        {
            throw new SocialSettingException(HttpStatusCode.Conflict, "Vote already exist for post.");
        }

        _apiDbContext.Votes.Remove(existingVoteEntity);

        await _apiDbContext.SaveChangesAsync();
    }

    public Task UpVoteCommentAsync(string postId, UserEntity currentUser)
    {
        var postGuid = GuidExtensions.ParseOrThrow(postId);
        throw new NotImplementedException();
    }

    public Task DownVoteCommentAsync(string postId, UserEntity currentUser)
    {
        throw new NotImplementedException();
    }

    private Task VoteForCommentAsync(string postId, UserEntity currentUser, VoteDirection voteDirection)
    {
        throw new NotImplementedException();
    }
    
    public Task RemoveVoteFromCommentAsync(string postId, UserEntity currentUser)
    {
        throw new NotImplementedException();
    }
}