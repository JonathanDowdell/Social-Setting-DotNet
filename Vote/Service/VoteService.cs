using System.Net;
using Microsoft.EntityFrameworkCore;
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

    /// <summary> The UpVotePostAsync function is used to upvote a post.</summary>
    ///
    /// <param name="postId"> The post to down vote</param>
    /// <param name="currentUser"> The user who is voting</param>
    ///
    /// <returns> A task.</returns>
    public async Task UpVotePostAsync(string postId, UserEntity currentUser)
    {
        await VoteForPostAsync(postId, currentUser, VoteDirection.Up);
    }

    /// <summary> The DownVotePostAsync function downvotes a post.</summary>
    ///
    /// <param name="postId"> The post to upvote.</param>
    /// <param name="currentUser"> The user that is voting</param>
    ///
    /// <returns> A task.</returns>
    public async Task DownVotePostAsync(string postId, UserEntity currentUser)
    {
        await VoteForPostAsync(postId, currentUser, VoteDirection.Down);
    }

    /// <summary> The VoteForPostAsync function is used to vote for a post.
    /// It takes in the current user, and the direction of their vote as parameters.
    /// It then checks if there is an existing vote for that post by that user, and if so it throws an exception
    /// with a status code of 409 (conflict). If not it removes any opposite votes from
    /// the post before adding this new one.</summary>
    ///
    /// <param name="postId"> The post id.</param>
    /// <param name="currentUser"> The user who is voting</param>
    /// <param name="voteDirection"> </param>
    ///
    /// <returns> A task.</returns>
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

    /// <summary> The RemoveOppositeVoteFromPostAsync function removes the opposite vote from a post.</summary>
    ///
    /// <param name="currentDirection"> The vote direction of the current user</param>
    /// <param name="currentUser"> The user who is voting</param>
    /// <param name="postGuid"> /// the guid of the post to be voted on.
    /// </param>
    ///
    /// <returns> A task.</returns>
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

    /// <summary> The RemoveVoteFromPostAsync function removes a vote from the PostVotes table.</summary>
    ///
    /// <param name="postId"> The post id.</param>
    /// <param name="currentUser"> </param>
    ///
    /// <returns> A task that represents the asynchronous operation.</returns>
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

    /// <summary> The UpVoteCommentAsync function upvotes a comment.</summary>
    ///
    /// <param name="commentId"> The comment to downvote</param>
    /// <param name="currentUser"> The user who is voting</param>
    ///
    /// <returns> A task.</returns>
    public async Task UpVoteCommentAsync(string commentId, UserEntity currentUser)
    {
        await VoteForCommentAsync(commentId, currentUser, VoteDirection.Up);
    }

    /// <summary> The DownVoteCommentAsync function downvotes a comment.</summary>
    ///
    /// <param name="commentId"> The id of the comment to be voted on.</param>
    /// <param name="currentUser"> The user who is voting</param>
    ///
    /// <returns> A task.</returns>
    public async Task DownVoteCommentAsync(string commentId, UserEntity currentUser)
    {
        await VoteForCommentAsync(commentId, currentUser, VoteDirection.Down);
    }

    /// <summary> The RemoveVoteFromCommentAsync function removes a vote from the comment with the given id.</summary>
    ///
    /// <param name="voteId"> The id of the comment to be updated</param>
    /// <param name="currentUser"> The user who is voting</param>
    ///
    /// <returns> A task.</returns>
    public async Task RemoveVoteFromCommentAsync(string voteId, UserEntity currentUser)
    {
        var voteGuid = GuidExtensions.ParseOrThrow(voteId);
        var voteEntity = await _apiDbContext.CommentVotes
            .Include(vote => vote.User)
            .FirstOrDefaultAsync(vote => vote.Id.Equals(voteGuid));
        if (voteEntity == null) throw new SocialSettingException(HttpStatusCode.NotFound, "Vote Not Found.");
        if (!voteEntity.User.Id.Equals(currentUser.Id)) throw new SocialSettingException(HttpStatusCode.NotAcceptable, "You are not the owner of this comment.");
        _apiDbContext.Remove(voteEntity);
        await _apiDbContext.SaveChangesAsync();
    }

    /// <summary> The VoteForCommentAsync function is used to vote for a comment.
    /// It takes in the current user, and the direction of their vote as parameters.
    /// It then checks if there is an existing vote from that user on that comment,
    /// and if so it throws an exception with a status code of 409 (conflict).
    /// If not it removes any opposite votes from the same user on that comment before adding a new one.</summary>
    ///
    /// <param name="commentId"> The comment id.</param>
    /// <param name="currentUser"> The user who is voting</param>
    /// <param name="voteDirection"> ///     the vote direction.
    /// </param>
    ///
    /// <returns> A task object.</returns>
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

    /// <summary> The RemoveOppositeVoteFromCommentAsync function removes the opposite vote from a comment.</summary>
    ///
    /// <param name="currentDirection"> The direction of the vote</param>
    /// <param name="currentUser"> </param>
    /// <param name="commentGuid"> /// the guid of the comment to be voted on.
    /// </param>
    ///
    /// <returns> A task</returns>
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