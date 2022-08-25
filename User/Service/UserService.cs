using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Npgsql;
using Social_Setting.Database;
using Social_Setting.Exception;
using Social_Setting.User.Data;
using Social_Setting.User.Model;

namespace Social_Setting.User.Service;

public class UserService: IUserService
{

    private readonly ApplicationApiDbContext _apiDbContext;

    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(ApplicationApiDbContext apiDbContext, IHttpContextAccessor httpContextAccessor)
    {
        _apiDbContext = apiDbContext;
        _httpContextAccessor = httpContextAccessor;
    }
    
    /// <summary> The SignUpUserAsync function creates a new user in the database.</summary>
    ///
    /// <param name="signUpUserRequest"> The sign up user request.</param>
    ///
    /// <returns> A userentity.</returns>
    public async Task<UserEntity> SignUpUserAsync(SignUpUserRequest signUpUserRequest)
    {
        using var hmac = new HMACSHA512();
        var userPassword = signUpUserRequest.Password;
        var bytes = System.Text.Encoding.UTF8.GetBytes(userPassword);
        var passwordSalt = hmac.Key;
        var passwordHash = await hmac.ComputeHashAsync(new MemoryStream(bytes));

        var role = new RoleEntity { Id = Guid.NewGuid(), Role = Roles.User.ToString()};
        var roles = new HashSet<RoleEntity> { role };
        var userEntity = new UserEntity
        {
            Id = Guid.NewGuid(), Email = signUpUserRequest.Email, Username = signUpUserRequest.Username,
            CreationDate = DateTime.Now, PasswordHash = passwordHash, PasswordSalt = passwordSalt, Roles = roles
        };
        var savedUser = await _apiDbContext.Users.AddAsync(userEntity);
        try
        {
            await _apiDbContext.SaveChangesAsync();
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException is PostgresException { SqlState: "23505" })
            {
                throw new SocialSettingException(HttpStatusCode.Conflict, "Email already in use.");
            }
            throw new SocialSettingException(HttpStatusCode.InternalServerError, e.Message);
        }
        catch (System.Exception e)
        {
            throw new SocialSettingException(HttpStatusCode.InternalServerError, e.Message);
        }

        return savedUser.Entity;
    }

    /// <summary> The SignInUserAsync function is used to sign in a user. It takes the SignInUserRequest object and returns the UserEntity.</summary>
    ///
    /// <param name="signInUserRequest"> The sign in user request.</param>
    ///
    /// <returns> A userentity.</returns>
    public async Task<UserEntity> SignInUserAsync(SignInUserRequest signInUserRequest)
    {
        var user = await _apiDbContext.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(user => user.Email == signInUserRequest.Email);

        if (user == null)
        {
            throw new SocialSettingException(HttpStatusCode.Unauthorized, "Unauthorized Request");
        }

        using var hmac = new HMACSHA512(user.PasswordSalt);
        var userPassword = signInUserRequest.Password;
        var bytes = System.Text.Encoding.UTF8.GetBytes(userPassword);
        var computedHash = await hmac.ComputeHashAsync(new MemoryStream(bytes));

        var matchingPasswords = computedHash.SequenceEqual(user.PasswordHash);

        if (!matchingPasswords)
        {
            throw new SocialSettingException(HttpStatusCode.Unauthorized, "Unauthorized Request");
        }

        return user;
    }


    /// <summary> The GetAllUsersAsync function returns a list of all users in the database.</summary>
    ///
    ///
    /// <returns> An ienumerable of userentity objects.</returns>
    public async Task<IEnumerable<UserEntity>> GetAllUsersAsync()
    {
        var userEntities = await _apiDbContext.Users
            .Include(u => u.Roles).ToListAsync();
        return userEntities;
    }

    /// <summary> The GetCurrentUserAsync function retrieves the current user from the database.</summary>
    ///
    /// <param name="includeRelations"> Include relations in the response</param>
    ///
    /// <returns> A userentity object.</returns>
    public async Task<UserEntity> GetCurrentUserAsync(bool includeRelations = false)
    {
        if (_httpContextAccessor.HttpContext == null)
            throw new SocialSettingException(HttpStatusCode.Unauthorized, "Unauthorized Request");
        
        var primaryId = _httpContextAccessor.HttpContext.User
            .FindFirstValue(ClaimTypes.PrimarySid);

        if (primaryId == null)
            throw new SocialSettingException(HttpStatusCode.Unauthorized, "Unauthorized Request");
        
        UserEntity? userEntity;

        if (includeRelations)
        {
            userEntity = await _apiDbContext.Users
                .Include(user => user.Roles)
                .FirstOrDefaultAsync(user => user.Id.Equals(Guid.Parse(primaryId)));
        }
        else
        {
            userEntity = await _apiDbContext.Users
                .FirstOrDefaultAsync(user => user.Id.Equals(Guid.Parse(primaryId)));
        }
        

        if (userEntity == null)
            throw new SocialSettingException(HttpStatusCode.Unauthorized, "Unauthorized Request");
            
        return userEntity;
    }
}