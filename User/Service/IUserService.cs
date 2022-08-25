using Social_Setting.User.Data;
using Social_Setting.User.Model;

namespace Social_Setting.User.Service;

public interface IUserService
{
    /// <summary> The SignUpUserAsync function creates a new user in the database.</summary>
    ///
    /// <param name="signUpUserRequest"> The sign up user request.</param>
    ///
    /// <returns> A userentity.</returns>
    public Task<UserEntity> SignUpUserAsync(SignUpUserRequest signUpUserRequest);
    
    /// <summary> The SignInUserAsync function is used to sign in a user. It takes the SignInUserRequest object and returns the UserEntity.</summary>
    ///
    /// <param name="signInUserRequest"> The sign in user request.</param>
    ///
    /// <returns> A userentity.</returns>
    public Task<UserEntity> SignInUserAsync(SignInUserRequest signInUserRequest);
    
    /// <summary> The GetAllUsersAsync function returns a list of all users in the database.</summary>
    ///
    ///
    /// <returns> An ienumerable of userentity objects.</returns>
    public Task<IEnumerable<UserEntity>> GetAllUsersAsync();
    
    /// <summary> The GetCurrentUserAsync function retrieves the current user from the database.</summary>
    ///
    /// <param name="includeRelations"> Include relations in the response</param>
    ///
    /// <returns> A userentity object.</returns>
    public Task<UserEntity> GetCurrentUserAsync(bool includeRelations = false);
}