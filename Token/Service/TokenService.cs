using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Social_Setting.Token.Model;
using Social_Setting.User.Data;

namespace Social_Setting.Token.Service;

public class TokenService: ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    /// <summary> The CreateServerToken function creates a JWT token for the user.</summary>
    ///
    /// <param name="userEntity"> </param>
    ///
    /// <returns> A tokenResponse object.</returns>
    public TokenResponse CreateServerToken(UserEntity userEntity)
    {
        var claims = userEntity.Roles
            .Select(role => new Claim(ClaimTypes.Role, role.Role))
            .ToArray()
            .Append(new Claim(ClaimTypes.PrimarySid, userEntity.Id.ToString()));
        
        var secretToken = _configuration.GetSection("AppSettings:Token").Value;
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretToken));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var serverTokenExpireDate = DateTime.Now.AddDays(1);
        
        var serverToken = new JwtSecurityToken(
            claims: claims, 
            expires: serverTokenExpireDate,
            signingCredentials: cred
        );
        
        var serverJwt = new JwtSecurityTokenHandler().WriteToken(serverToken);
        
        return new TokenResponse(serverJwt, serverTokenExpireDate);
    }
}