namespace Social_Setting.Token.Model;

public class TokenResponse
{
    public TokenResponse(string token, string refreshToken, DateTime expireDate)
    {
        Token = token;
        RefreshToken = refreshToken;
        ExpireDate = expireDate;
    }

    public string Token { get; set; }

    public string RefreshToken { get; set; }
    
    public DateTime ExpireDate { get; set; }
}