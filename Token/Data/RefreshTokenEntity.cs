using Social_Setting.User.Data;

namespace Social_Setting.Token.Data;

public class RefreshTokenEntity
{
    public Guid Id { get; set; }
    
    public string Token { get; set; } = string.Empty;

    public DateTime Created { get; set; } = DateTime.Now;

    public DateTime Expires { get; set; }

    public UserEntity User { get; set; }
}