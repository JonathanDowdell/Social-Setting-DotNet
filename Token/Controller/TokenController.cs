using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Setting.Token.Model;
using Social_Setting.Token.Service;

namespace Social_Setting.Token.Controller;

[ApiController]
[Route("token")]
public class TokenController: Microsoft.AspNetCore.Mvc.Controller
{

    private readonly ITokenService _tokenService;

    public TokenController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    /// <summary> The RefreshToken function exchanges a refresh token for an access token.</summary>
    ///
    /// <param name="refreshTokenRequest"> Refreshtokenrequest</param>
    ///
    /// <returns> A tokenresponse object.</returns>
    [HttpPost("refresh"), AllowAnonymous]
    public async Task<TokenResponse> RefreshToken(RefreshTokenRequest refreshTokenRequest)
    {
        return await _tokenService.ExchangeRefreshToken(refreshTokenRequest.RefreshToken);
    }
}