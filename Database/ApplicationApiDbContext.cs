using Microsoft.EntityFrameworkCore;
using Social_Setting.Comment.Data;
using Social_Setting.Post.Data;
using Social_Setting.Setting.Data;
using Social_Setting.User.Data;
using Social_Setting.Vote.Data;
using Social_Setting.Vote.Model;

namespace Social_Setting.Database;

public class ApplicationApiDbContext: DbContext
{
    public ApplicationApiDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<UserEntity> Users { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<SettingEntity> Settings { get; set; }
    public DbSet<PostEntity> Posts { get; set; }
    public DbSet<CommentEntity> Comments { get; set; }
    public DbSet<PostVoteEntity> PostVotes { get; set; }
    public DbSet<CommentVoteEntity> CommentVotes { get; set; }
}