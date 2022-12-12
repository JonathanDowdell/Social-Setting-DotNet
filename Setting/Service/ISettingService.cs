using System.Collections;
using Social_Setting.Setting.Data;
using Social_Setting.Setting.Model;
using Social_Setting.User.Data;
using Social_Setting.Utils.Model;

namespace Social_Setting.Setting.Service;

public interface ISettingService
{
    /// <summary> The GetSettingByNameAsync function returns a setting entity by name.</summary>
    ///
    /// <param name="name"> The name of the setting to update</param>
    ///
    /// <returns> A settingentity object.</returns>
    public Task<SettingEntity> GetSettingByNameAsync(string name);
    
    /// <summary> The GetSettingsByNameAsync function returns a list of SettingEntity objects that match the name provided.</summary>
    ///
    /// <param name="name"> The name of the setting to be retrieved.</param>
    ///
    /// <returns> An ienumerable of settingentity objects.</returns>
    public Task<IEnumerable<SettingEntity>> GetSettingsByNameAsync(string name);

    /// <summary> The CreateSettingAsync function creates a new SettingEntity object and adds it to the database.</summary>
    ///
    /// <param name="currentUser"> The user who is currently logged in.</param>
    /// <param name="createSettingRequest"> The request object containing the title and description of the new setting.
    /// </param>
    ///
    /// <returns> A settingentity</returns>
    public Task<SettingEntity> CreateSettingAsync(UserEntity currentUser, CreateSettingRequest createSettingRequest);

    public Task<IEnumerable<SettingEntity>> GetAllSubSettings(Pagination pagination);
}