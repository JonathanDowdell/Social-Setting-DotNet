using Social_Setting.User.Data;

namespace Social_Setting.User.Model;

public class UserResponse
{
    public UserResponse(UserEntity userEntity)
    {
        this.Email = userEntity.Email;
        this.Username = userEntity.Username;
        this.CreationDate = userEntity.CreationDate.ToString("MM/dd/yyyy HH:mm:ss");
    }
    
    public UserResponse(string email, string username, string creationDate)
    {
        Email = email;
        Username = username;
        CreationDate = creationDate;
    }

    public string Email { get; set; }
    public string Username { get; set; }
    public string CreationDate { get; set; }
 
}