using Microsoft.AspNetCore.Mvc;

namespace Social_Setting.Token.Controller;

[ApiController]
[Route("token")]
public class TokenController: Microsoft.AspNetCore.Mvc.Controller
{
    // TODO: Finish Refresh Token Function
    [HttpPost("refresh")]
    public void RefreshToken()
    {
        
    }
}