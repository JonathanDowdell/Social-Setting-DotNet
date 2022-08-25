namespace Social_Setting.User.Model;

public class SignUpUserRequest
{

    public SignUpUserRequest(string email, string username, string password)
    {
        this.Email = email;
        this.Username = username;
        this.Password = password;
    }
    
    public string Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}