using Social_Setting.Token.Model;
using Social_Setting.User.Data;

namespace Social_Setting.Token.Service;

public interface ITokenService
{
    /// <summary> The CreateServerToken function creates a JWT token for the user.</summary>
    ///
    /// <param name="userEntity"> </param>
    ///
    /// <returns> A tokenResponse object.</returns>
    TokenResponse CreateServerToken(UserEntity userEntity);
}