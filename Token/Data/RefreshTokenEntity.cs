using Social_Setting.User.Data;

namespace Social_Setting.Token.Data;

public class RefreshTokenEntity
{
    public Guid Id { get; set; }
    
    public DateTime Created { get; set; } = DateTime.Now;

    public UserEntity User { get; set; }
}