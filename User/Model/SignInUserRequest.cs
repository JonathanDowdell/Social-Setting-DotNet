namespace Social_Setting.User.Model;

public class SignInUserRequest
{
    public SignInUserRequest(string email, string password)
    {
        Email = email;
        this.Password = password;
    }

    public string Email { get; set; }
    public string Password { get; set; } 
}