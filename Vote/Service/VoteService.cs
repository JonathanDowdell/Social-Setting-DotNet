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

        var existingVoteEntity = await _apiDbContext.PostVotes
            .FirstOrDefaultAsync(vote => 
                vote.User.Id.Equals(currentUser.Id) && 
                vote.Post.Id.Equals(postGuid) 
                && vote.VoteDirection.Equals(voteDirection));

        if (existingVoteEntity != null) throw new SocialSettingException(HttpStatusCode.Conflict, "Vote already exist for post.");

        var post = await _apiDbContext.Posts
            .FindAsync(postGuid);

        if (post == null) throw new SocialSettingException(HttpStatusCode.NotFound, "Post Not Found.");

        await RemoveOppositeVoteFromPostAsync(voteDirection, currentUser, postGuid);
        
        var voteEntity = new PostVoteEntity()
        {
            Id = Guid.NewGuid(),
            Post = post,
            User = currentUser,
            VoteDirection = voteDirection
        };

        _apiDbContext.PostVotes.Add(voteEntity);
        
        await _apiDbContext.SaveChangesAsync();
        
    }

    private async Task RemoveOppositeVoteFromPostAsync(VoteDirection currentDirection, UserEntity currentUser, Guid postGuid)
    {
        PostVoteEntity? existingVoteEntity;
        if (currentDirection.Equals(VoteDirection.Down))
        {
            existingVoteEntity = await _apiDbContext.PostVotes
                .FirstOrDefaultAsync(vote => 
                    vote.User.Id.Equals(currentUser.Id) && 
                    vote.Post.Id.Equals(postGuid) 
                    && vote.VoteDirection.Equals(VoteDirection.Up));
        }
        else
        {
            existingVoteEntity = await _apiDbContext.PostVotes
                .FirstOrDefaultAsync(vote => 
                    vote.User.Id.Equals(currentUser.Id) && 
                    vote.Post.Id.Equals(postGuid) 
                    && vote.VoteDirection.Equals(VoteDirection.Down));
        }

        if (existingVoteEntity != null)
        {
            _apiDbContext.PostVotes.Remove(existingVoteEntity);
        }
    }

    public async Task RemoveVoteFromPostAsync(string postId, UserEntity currentUser)
    {
        var postGuid = GuidExtensions.ParseOrThrow(postId);
        
        var existingVoteEntity = await _apiDbContext.PostVotes
            .FirstOrDefaultAsync(vote => 
                vote.User.Id.Equals(currentUser.Id) && 
                vote.Post.Id.Equals(postGuid));

        if (existingVoteEntity == null) throw new SocialSettingException(HttpStatusCode.Conflict, "Vote already exist for post.");

        _apiDbContext.PostVotes.Remove(existingVoteEntity);

        await _apiDbContext.SaveChangesAsync();
    }

    public async Task UpVoteCommentAsync(string commentId, UserEntity currentUser)
    {
        await VoteForCommentAsync(commentId, currentUser, VoteDirection.Up);
    }

    public async Task DownVoteCommentAsync(string commentId, UserEntity currentUser)
    {
        await VoteForCommentAsync(commentId, currentUser, VoteDirection.Down);
    }

    public Task RemoveVoteFromCommentAsync(string commentId, UserEntity currentUser)
    {
        throw new NotImplementedException();
    }

    private async Task VoteForCommentAsync(string commentId, UserEntity currentUser, VoteDirection voteDirection)
    {
        var commentGuid = GuidExtensions.ParseOrThrow(commentId);

        var existingVoteEntity = await _apiDbContext.CommentVotes
            .FirstOrDefaultAsync(vote => 
                vote.User.Id.Equals(currentUser.Id) &&
                vote.Comment.Id.Equals(commentGuid) &&
                vote.VoteDirection.Equals(voteDirection)
            );

        if (existingVoteEntity != null) throw new SocialSettingException(HttpStatusCode.Conflict, "Vote already exist for comment.");

        var commentEntity = await _apiDbContext.Comments
            .FindAsync(commentGuid);

        if (commentEntity == null) throw new SocialSettingException(HttpStatusCode.NotFound, "Comment Not Found.");

        await RemoveOppositeVoteFromCommentAsync(voteDirection, currentUser, commentGuid);

        var commentVoteEntity = new CommentVoteEntity()
        {
            Id = Guid.NewGuid(),
            Comment = commentEntity,
            User = currentUser,
            VoteDirection = voteDirection
        };

        _apiDbContext.CommentVotes.Add(commentVoteEntity);

        await _apiDbContext.SaveChangesAsync();
    }

    private async Task RemoveOppositeVoteFromCommentAsync(VoteDirection currentDirection, UserEntity currentUser, Guid commentGuid)
    {
        
        CommentVoteEntity? existingVoteEntity;
        if (currentDirection.Equals(VoteDirection.Down))
        {
            existingVoteEntity = await _apiDbContext.CommentVotes
                .FirstOrDefaultAsync(vote => 
                    vote.User.Id.Equals(currentUser.Id) && 
                    vote.Comment.Id.Equals(commentGuid) 
                    && vote.VoteDirection.Equals(VoteDirection.Up));
        }
        else
        {
            existingVoteEntity = await _apiDbContext.CommentVotes
                .FirstOrDefaultAsync(vote => 
                    vote.User.Id.Equals(currentUser.Id) && 
                    vote.Comment.Id.Equals(commentGuid) 
                    && vote.VoteDirection.Equals(VoteDirection.Down));
        }

        if (existingVoteEntity != null)
        {
            _apiDbContext.CommentVotes.Remove(existingVoteEntity);
        }
    }
}