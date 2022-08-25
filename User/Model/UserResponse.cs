using Social_Setting.User.Data;

namespace Social_Setting.User.Model;

public class UserResponse
{
    public UserResponse(UserEntity userEntity)
    {
        this.Email = userEntity.Email;
        this.Username = userEntity.Username;
        this.CreationDate = userEntity.CreationDate;
    }
    
    public UserResponse(string email, string username, DateTime creationDate)
    {
        Email = email;
        Username = username;
        CreationDate = creationDate;
    }

    public string Email { get; set; }
    public string Username { get; set; }
    public DateTime CreationDate { get; set; }
 
}