using System.Net;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Social_Setting.Database;
using Social_Setting.Exception;
using Social_Setting.Post.Data;
using Social_Setting.Setting.Data;
using Social_Setting.Setting.Model;
using Social_Setting.User.Data;
using Social_Setting.Utils.Model;

namespace Social_Setting.Setting.Service;

public class SettingService : ISettingService
{
    
    private readonly ApplicationApiDbContext _apiDbContext;

    public SettingService(ApplicationApiDbContext apiDbContext)
    {
        _apiDbContext = apiDbContext;
    }

    /// <summary> The CreateSettingAsync function creates a new SettingEntity object and adds it to the database.</summary>
    ///
    /// <param name="currentUser"> The user who is currently logged in.</param>
    /// <param name="createSettingRequest"> The request object containing the title and description of the new setting.
    /// </param>
    ///
    /// <returns> A settingentity</returns>
    public async Task<SettingEntity> CreateSettingAsync(UserEntity currentUser, CreateSettingRequest createSettingRequest)
    {
        var settingEntity = new SettingEntity
        {
            Id = Guid.NewGuid(),
            Title = createSettingRequest.Title,
            Description = createSettingRequest.Description,
            CreationDate = DateTime.Now,
            User = currentUser,
            Post = new List<PostEntity>()
        };

        var createdSubSettingEntity = await _apiDbContext.Settings.AddAsync(settingEntity);
        
        try
        {
            await _apiDbContext.SaveChangesAsync();
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException is PostgresException { SqlState: "23505" })
            {
                throw new SocialSettingException(HttpStatusCode.Conflict, "Setting Title already used.");
            }
            throw new SocialSettingException(HttpStatusCode.InternalServerError, e.Message);
        }
        catch (System.Exception e)
        {
            throw new SocialSettingException(HttpStatusCode.InternalServerError, e.Message);
        }
        
        return createdSubSettingEntity.Entity;
    }

    /// <summary> The GetSettingByNameAsync function returns a setting entity by name.</summary>
    ///
    /// <param name="name"> The name of the setting to update</param>
    ///
    /// <returns> A settingentity object.</returns>
    public async Task<SettingEntity> GetSettingByNameAsync(string name)
    {
        var settingEntity = await _apiDbContext.Settings
            .FirstOrDefaultAsync(setting => String.Equals(setting.Title, name, StringComparison.CurrentCultureIgnoreCase));
        if (settingEntity == null)
        {
            throw new SocialSettingException(HttpStatusCode.NotFound, $"{name} not found");
        }
        return settingEntity;
    }

    /// <summary> The GetSettingsByNameAsync function returns a list of SettingEntity objects that match the name provided.</summary>
    ///
    /// <param name="name"> The name of the setting to be retrieved.</param>
    ///
    /// <returns> An ienumerable of settingentity objects.</returns>
    public async Task<IEnumerable<SettingEntity>> GetSettingsByNameAsync(string name)
    {
        var settingEntities = await _apiDbContext.Settings
            .Where(setting => setting.Title.ToLower().Contains(name.ToLower()))
            .ToArrayAsync();
        return settingEntities;
    }
    
    /// <summary> The GetAllSubSettings function returns all sub settings for a given setting.</summary>
    ///
    /// <param name="pagination"> Pagination object</param>
    ///
    /// <returns> A list of settingentity objects.</returns>
    public async Task<IEnumerable<SettingEntity>> GetAllSubSettings(Pagination pagination)
    {
        var settingEntities = await _apiDbContext.Settings
            .OrderBy(subSetting => subSetting.Title)
            .Skip(pagination.Skip)
            .Take(pagination.Take)
            .ToArrayAsync();

        return settingEntities;
    }

}