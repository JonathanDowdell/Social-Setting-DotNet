using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Social_Setting.Database;
using Social_Setting.Exception;
using Social_Setting.Extension;
using Social_Setting.Token.Data;
using Social_Setting.Token.Model;
using Social_Setting.User.Data;

namespace Social_Setting.Token.Service;

public class TokenService: ITokenService
{
    private readonly IConfiguration _configuration;

    private readonly ApplicationApiDbContext _apiDbContext;
    
    public TokenService(IConfiguration configuration, ApplicationApiDbContext apiDbContext)
    {
        _configuration = configuration;
        _apiDbContext = apiDbContext;
    }

    /// <summary> The CreateServerToken function creates a JWT token for the server to use. It takes in a UserEntity object and returns a TokenResponse object.</summary>
    ///
    /// <param name="currentUser"> The user to create the refresh token for</param>
    ///
    /// <returns> A tokenresponse object with the server token, refresh token and expiration date.</returns>
    public async Task<TokenResponse> CreateServerToken(UserEntity currentUser)
    {
        var claims = currentUser.Roles
            .Select(role => new Claim(ClaimTypes.Role, role.Role))
            .ToArray()
            .Append(new Claim(ClaimTypes.PrimarySid, currentUser.Id.ToString()));
        
        var secretToken = _configuration.GetSection("AppSettings:Token").Value;
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretToken));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var serverTokenExpireDate = DateTime.Now.AddDays(1);
        
        var serverToken = new JwtSecurityToken(
            claims: claims, 
            expires: serverTokenExpireDate,
            signingCredentials: cred
        );
        
        var serverTokenJwt = new JwtSecurityTokenHandler().WriteToken(serverToken);
        
        var refreshTokenJwt = await CreateRefreshTokenJwt(currentUser, cred);

        return new TokenResponse(serverTokenJwt, refreshTokenJwt, serverTokenExpireDate);
    }

    /// <summary> The CreateRefreshTokenJwt function creates a RefreshToken and JWT token.</summary>
    /// <param name="currentUser"> </param>
    /// <param name="credentials"></param>
    /// <returns> RefreshTokenJwt.</returns>
    private async Task<string> CreateRefreshTokenJwt(UserEntity currentUser, SigningCredentials credentials)
    {
        var refreshTokenExpireDate = DateTime.Now.AddDays(7);
        var refreshTokeId = Guid.NewGuid();
        var refreshTokenEntity = new RefreshTokenEntity()
        {
            Id = refreshTokeId,
            User = currentUser,
        };
        _apiDbContext.RefreshTokens.Add(refreshTokenEntity);
        await _apiDbContext.SaveChangesAsync();
        var refreshToken = new JwtSecurityToken(
            claims: new[] { new Claim("RefreshToken", refreshTokeId.ToString()) },
            expires: refreshTokenExpireDate,
            signingCredentials: credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(refreshToken);
    }

    

    /// <summary> The ExchangeRefreshToken function exchanges a refresh token for a new access token.</summary>
    ///
    /// <param name="refreshToken"> The refresh token.</param>
    ///
    /// <returns> A tokenresponse object.</returns>
    public async Task<TokenResponse> ExchangeRefreshToken(string refreshToken)
    {
        var key = _configuration.GetSection("AppSettings:Token").Value;
        if (key == null) throw new SocialSettingException(HttpStatusCode.InternalServerError, "Key Not Found.");
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters()
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false,
            ValidateAudience = false,
            ValidateIssuer = false
        };
        var tokenValidationResult = await jwtSecurityTokenHandler.ValidateTokenAsync(refreshToken, validationParameters);
        if (!tokenValidationResult.IsValid)
            throw new SocialSettingException(HttpStatusCode.Unauthorized, "Invalid Token.");
        var refreshTokenStringId = tokenValidationResult.Claims["RefreshToken"].ToString();
        if (refreshTokenStringId == null)
            throw new SocialSettingException(HttpStatusCode.Unauthorized, "Refresh Token Id Not Found.");
        var refreshTokenGuid = GuidExtensions.ParseOrThrow(refreshTokenStringId);
        var foundRefreshToken = await _apiDbContext.RefreshTokens
            .Include(token => token.User).ThenInclude(user => user.Roles)
            .FirstOrDefaultAsync(token => token.Id.Equals(refreshTokenGuid));
        if (foundRefreshToken == null) throw new SocialSettingException(HttpStatusCode.NotFound, "User Not Found.");
        var user = foundRefreshToken.User;
        _apiDbContext.RefreshTokens.Remove(foundRefreshToken);
        return await CreateServerToken(user);
    }
}