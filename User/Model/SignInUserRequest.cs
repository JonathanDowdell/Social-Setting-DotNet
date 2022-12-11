using System.ComponentModel.DataAnnotations;

namespace Social_Setting.User.Model;

public class SignInUserRequest
{
    public SignInUserRequest(string username, string password)
    {
        Username = username;
        Password = password;
    }

    public string Username { get; set; }
    public string Password { get; set; } 
}