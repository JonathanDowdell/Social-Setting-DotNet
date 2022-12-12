using Social_Setting.Setting.Data;

namespace Social_Setting.Setting.Model;

public class SettingResponse
{
    public SettingResponse(SettingEntity settingEntity)
    {
        this.Id = settingEntity.Id;
        this.Name = settingEntity.Title;
    }

    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
}